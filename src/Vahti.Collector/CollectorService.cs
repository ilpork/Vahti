using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vahti.Collector.Configuration;
using Vahti.Collector.DeviceScanner;
using Vahti.Shared;
using Vahti.Shared.Data;
using Vahti.Shared.TypeData;
using Vahti.Shared.Utils;

namespace Vahti.Collector
{
    /// <summary>
    /// Service to scan and collect data from devices and publish it using an MQTT client
    /// </summary>
    public class CollectorService : BackgroundService, IMqttClientConnectedHandler
    {        
        public const int MaxRepeatedReadFailCount = 10;

        private readonly ILogger<CollectorService> _logger;
        private readonly CollectorConfiguration _config;
        private readonly IDeviceScanner _deviceScanner;
        private readonly List<SensorDevice> _sensorDevices;
        private readonly List<SensorDeviceType> _sensorDeviceTypes;
        private readonly IManagedMqttClient _mqttClient;

        private bool _alreadyDisposed;

        public CollectorService(ILogger<CollectorService> logger, IOptions<CollectorConfiguration> config, IDeviceScanner deviceScanner, IManagedMqttClient mqttClient)
        {
            _logger = logger;
            _config = config.Value;
            _sensorDevices = config.Value.SensorDevices;
            _sensorDeviceTypes = config.Value.SensorDeviceTypes;
            _deviceScanner = deviceScanner;
            _mqttClient = mqttClient;
        }

        protected virtual void Dispose(bool explicitCall)
        {
            if (!_alreadyDisposed)
            {
                base.Dispose();
                if (explicitCall)
                {
                }
                _alreadyDisposed = true;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_config.CollectorEnabled)
            {
                return;
            }

            _logger.LogInformation("Started");

            var adapterName = _config.BluetoothAdapterName ?? "[first found]";            

            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId(GetType().Name)
                .WithTcpServer(_config.MqttServerAddress)
                .Build())
                .Build();


            var measurements = new List<MeasurementData>();

            // Publish information about known sensors and sensor types always when client (re)connects
            _mqttClient.ConnectedHandler = this;

            await _mqttClient.StartAsync(options);

            while (!_mqttClient.IsConnected)
            {
                await Task.Delay(1000);
            }

            try
            {
                int consecutiveReadFailCount = 0;

                // Main loop
                while (!stoppingToken.IsCancellationRequested)
                {
                    measurements.Clear();

                    // First scan the devices
                    try
                    {
                        measurements.AddRange(await _deviceScanner.GetDeviceDataAsync(_sensorDevices));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{DateTime.Now}: Reading device data failed: {ex.Message}, {ex.StackTrace}");
                        consecutiveReadFailCount++;

                        // Break from then loop if the problem persists and configured to stop on repeated errors
                        if (consecutiveReadFailCount >= MaxRepeatedReadFailCount && _config.StopOnRepeatedErrors)
                        {
                            break;
                        }

                        continue;
                    }


                    // Publish data to server
                    int publishCounter = 0;
                    foreach (var measurement in measurements)
                    {
                        var sensorDevice = _sensorDevices.First(d => d.Id.Equals(measurement.SensorDeviceId, StringComparison.OrdinalIgnoreCase));
                        var topic = $"{Constant.TopicMeasurement}/{sensorDevice.Location}/{sensorDevice.Id}/{measurement.SensorId}";

                        await _mqttClient.PublishAsync(
                            new MqttApplicationMessageBuilder()
                            .WithPayload(measurement.Value)
                            .WithRetainFlag()
                            .WithTopic(topic)
                            .Build(), CancellationToken.None);

                        publishCounter++;
                    }

                    _logger.LogInformation($"{DateTime.Now}: Published data for {publishCounter} measurement(s)");
                    consecutiveReadFailCount = 0;
                    await Task.Delay(TimeSpan.FromSeconds(_config.ScanIntervalSeconds), stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Reading or publishing sensor data failed: {ex.Message}");
            }
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs e)
        {
            foreach (var sensorDeviceType in _sensorDeviceTypes)
            {
                await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                    .WithPayload(MqttMessageHelper.SerializePayload(sensorDeviceType))
                    .WithRetainFlag().WithTopic(MqttMessageHelper.GetSensorDeviceTypeTopic(sensorDeviceType.Id))
                    .Build(), CancellationToken.None);
            }

            foreach (var sensorDevice in _sensorDevices)
            {
                await _mqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                    .WithPayload(MqttMessageHelper.SerializePayload(sensorDevice))
                    .WithRetainFlag().WithTopic(MqttMessageHelper.GetSensorDeviceTopic(sensorDevice.Id))
                    .Build(), CancellationToken.None);
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
    }
}
