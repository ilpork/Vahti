using BleReaderNet.Device;
using BleReaderNet.Reader;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Vahti.BluetoothGw.Configuration;
using Vahti.BluetoothGw.DeviceScanner;
using Vahti.Shared;
using Vahti.Shared.Data;
using Vahti.Shared.TypeData;
using Vahti.Shared.Utils;

namespace Vahti.BluetoothGw
{
    /// <summary>
    /// Service to scan bluetooth devices and publish their data using a MQTT client
    /// </summary>
    public class BluetoothGwService : BackgroundService, IMqttClientConnectedHandler
    {
        public const int DeviceScanDuration = 10;
        public const int MaxRepeatedReadFailCount = 10;

        private readonly ILogger<BluetoothGwService> _logger;
        private readonly BluetoothGwConfiguration _config;
        private readonly IDeviceScanner _deviceScanner;
        private readonly List<SensorDevice> _sensorDevices;
        private readonly List<SensorDeviceType> _sensorDeviceTypes;
        private readonly IManagedMqttClient _mqttClient;
        
        private bool _alreadyDisposed;

        public BluetoothGwService(ILogger<BluetoothGwService> logger, IOptions<BluetoothGwConfiguration> config, IDeviceScanner deviceScanner, IManagedMqttClient mqttClient)
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
            if (!_config.BluetoothGwEnabled)
            {
                return;
            }

            _logger.LogInformation("Started");

            var adapterName = _config.AdapterName ?? "[first found]";
            _logger.LogInformation($"Using Bluetooth adapter {adapterName}");

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
                        _logger.LogDebug($"{DateTime.Now}: Scanning for {DeviceScanDuration} seconds...");
                        await _deviceScanner.ScanDevicesAsync(_config.AdapterName, DeviceScanDuration);

                        // Read device data
                        foreach (var sensorDevice in _sensorDevices)
                        {
                            var deviceMeasurements = await _deviceScanner.GetDeviceDataAsync(sensorDevice);
                            if (deviceMeasurements != null)
                            {
                                measurements.AddRange(deviceMeasurements);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Reading device data failed: " + ex.Message);
                        consecutiveReadFailCount++;

                        // Break from loop if the problem persists
                        if (consecutiveReadFailCount >= MaxRepeatedReadFailCount)
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
                        var topic= $"{Constant.TopicMeasurement}/{sensorDevice.Location}/{sensorDevice.Id}/{measurement.SensorId}";

                        await _mqttClient.PublishAsync(
                            new MQTTnet.MqttApplicationMessageBuilder()
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
                await _mqttClient.PublishAsync(new MQTTnet.MqttApplicationMessageBuilder()
                    .WithPayload(MqttMessageHelper.SerializePayload(sensorDeviceType))
                    .WithRetainFlag().WithTopic(MqttMessageHelper.GetSensorDeviceTypeTopic(sensorDeviceType.Id))
                    .Build(), CancellationToken.None);
            }

            foreach (var sensorDevice in _sensorDevices)
            {
                await _mqttClient.PublishAsync(new MQTTnet.MqttApplicationMessageBuilder()
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
