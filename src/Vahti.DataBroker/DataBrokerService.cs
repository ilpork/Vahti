using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vahti.DataBroker.Configuration;
using Vahti.DataBroker.DataBroker;

namespace Vahti.DataBroker
{
    /// <summary>
    /// Service to publish the measurement data sent by client to cloud
    /// </summary>
    public class DataBrokerService : BackgroundService
    {
        public const int MaxRepeatedFailCount = 10;

        private const int DefaultLoopInterval = 60; // How often run the main loop (in seconds)
        private const int HistoryDatabaseCleanupIntervalMinutes = 60 * 24; // Clean up history database of old data once per day

        private readonly ILogger<DataBrokerService> _logger;
        private readonly DataBrokerConfiguration _config;
        private readonly IDataBroker _dataBroker;        
        private readonly IManagedMqttClient _mqttClient;

        public DataBrokerService(ILogger<DataBrokerService> logger, IOptions<DataBrokerConfiguration> dataBrokerConfig, IManagedMqttClient mqttClient, 
            IDataBroker dataBroker)
        {
            _logger = logger;
            _config = dataBrokerConfig.Value;                        
            
            _mqttClient = mqttClient;
            _dataBroker = dataBroker;
        }

        internal int LoopInterval { get; set; } = DefaultLoopInterval;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_config.DataBrokerEnabled)
            {
                return;
            }

            _logger.LogInformation("Started");
            _logger.LogInformation($"Cloud publishing enabled: {_config.CloudPublishConfiguration.Enabled}");

            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId(GetType().Name)
                .WithTcpServer(_config.MqttServerAddress)
                .Build())
                .Build();                       

            await _mqttClient.SubscribeAsync("sensordevicetype/#");
            await _mqttClient.SubscribeAsync("sensordevice/#");
            await _mqttClient.SubscribeAsync("measurement/#");

            _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(MqttMessageReceived); 
            
            await _mqttClient.StartAsync(options);

            var minuteCounter = 0;
            int consecutiveReadFailCount = 0;            

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_config.CloudPublishConfiguration.Enabled)
                    {
                        if (minuteCounter % _config.CloudPublishConfiguration.UpdateIntervalMinutes == 0)
                        {
                            await _dataBroker.PublishCurrentData();                            
                        }

                        if (minuteCounter % _config.CloudPublishConfiguration.HistoryUpdateIntervalMinutes == 0)
                        {
                            await _dataBroker.PublishHistoryData();
                        }

                        if (minuteCounter % HistoryDatabaseCleanupIntervalMinutes == 0)
                        {
                            await _dataBroker.DeleteOldHistoryData();
                        }
                    }

                    await _dataBroker.SendAlerts();
                    consecutiveReadFailCount = 0;                    
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Stopped");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{DateTime.Now}: Error occured in Vahti.DataBroker: {ex.Message}, {ex.StackTrace}");
                    consecutiveReadFailCount++;

                    // Break from the loop if the problem persists and configured to stop on repeated errors
                    if (consecutiveReadFailCount >= MaxRepeatedFailCount && _config.StopOnRepeatedErrors)
                    {
                        break;
                    }
                    continue;
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(LoopInterval), stoppingToken);
                    minuteCounter++;
                }                
            }                            
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient != null)
            {
                _mqttClient.Dispose();
            }

            await base.StopAsync(cancellationToken);
        }

        private void MqttMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {            
            _logger.LogDebug($"{DateTime.Now}: {e.ApplicationMessage.Topic}, {e.ApplicationMessage.ConvertPayloadToString()}");
            _dataBroker.MqttMessageReceived(e.ApplicationMessage.Topic, e.ApplicationMessage.ConvertPayloadToString());
        }
    }
}
