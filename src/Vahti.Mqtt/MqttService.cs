using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet.Server;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vahti.Mqtt.Configuration;

namespace Vahti.Mqtt
{    
    public class MqttService : BackgroundService
    {
        private readonly ILogger<MqttService> _logger;        
        private readonly IMqttServer _server;
        private readonly MqttConfiguration _config;

        public MqttService(ILogger<MqttService> logger, IOptions<MqttConfiguration> mqttConfig)
        {
            _logger = logger;
            _config = mqttConfig.Value;
            _server = new MQTTnet.MqttFactory().CreateMqttServer();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_config.MqttServerEnabled)
            {
                return;
            }

            await _server.StartAsync(new MqttServerOptions());

            _server.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate((e) =>
            {
                _logger.LogInformation($"{DateTime.Now}: Client '{e.ClientId}' connected");
            });

            _server.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate((e) =>
            {
                _logger.LogInformation($"{DateTime.Now}: Client '{e.ClientId}' disconnected");
            });

            _logger.LogInformation("Started");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Stopped");
            }            
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _server.StopAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
