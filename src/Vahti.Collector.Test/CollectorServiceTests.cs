using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MQTTnet;
using MQTTnet.Client.Connecting;
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

namespace Vahti.Collector.Test
{
    /// <Summary>
    /// Unit tests for <see cref="CollectorService"/>
    /// </Summary>
    [TestClass]
    public class CollectorServiceTests
    {
        private Mock<ILogger<CollectorService>> _loggerMock;
        private Mock<IOptions<CollectorConfiguration>> _configMock;
        private Mock<IDeviceScanner> _deviceScannerMock;
        private Mock<IManagedMqttClient> _mqttClientMock;
        private ServiceProvider _serviceProvider;

        [TestInitialize]
        public void InitializeTest()
        {
            _loggerMock = new Mock<ILogger<CollectorService>>();
            _configMock = new Mock<IOptions<CollectorConfiguration>>();
            _deviceScannerMock = new Mock<IDeviceScanner>();
            _mqttClientMock = new Mock<IManagedMqttClient>();

            var services = new ServiceCollection();
            services.AddSingleton<CollectorService>();
            services.AddSingleton(l => _loggerMock.Object);
            services.AddSingleton(c => _configMock.Object);
            services.AddSingleton(d => _deviceScannerMock.Object);
            services.AddSingleton(d => _mqttClientMock.Object);

            _serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public async Task ExecuteAsync_PublishMeasurementData_CorrectlyPublished()
        {
            MqttApplicationMessage publishedMessage = null;
            var sensorDevice = new SensorDevice()
            {
                Id = "Device1",
                SensorDeviceTypeId = "DeviceType1"
            };

            var sensorDeviceType = new SensorDeviceType()
            {
                Id = "DeviceType1",
                Sensors = new List<Sensor>() {
                        new Sensor(){
                            Id = "Sensor1",
                        }
                    }
            };

            //Arrange
            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 0,
                SensorDevices = new List<SensorDevice>() { sensorDevice },
                SensorDeviceTypes = new List<SensorDeviceType>() { sensorDeviceType }
            };

            var measurementData = new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = sensorDeviceType.Sensors[0].Id, Timestamp = DateTime.Now, Value = "123" };
            var measurementList = new List<MeasurementData>() { measurementData };
            _mqttClientMock.Setup(m => m.IsConnected).Returns(true);
            _mqttClientMock.Setup(m => m.PublishAsync(It.IsAny<MqttApplicationMessage>(), CancellationToken.None)).Callback((MqttApplicationMessage message, CancellationToken ct) => { publishedMessage = message; });
            _configMock.Setup(c => c.Value).Returns(config);
            _deviceScannerMock.Setup(d => d.GetDeviceDataAsync(config.SensorDevices)).ReturnsAsync(measurementList);

            //Act      
            var btGwService = _serviceProvider.GetService<CollectorService>();
            var cts = new CancellationTokenSource();

#pragma warning disable CS4014
            // Run as fire & forget
            Task.Run(() => btGwService.StartAsync(cts.Token).ConfigureAwait(false));
#pragma warning restore CS4014

            await Task.Delay(300);
            cts.Cancel();

            var expectedMessage = $"{Constant.TopicMeasurement}/{sensorDevice.Location}/{sensorDevice.Id}/{measurementData.SensorId}";
            Assert.AreEqual(expectedMessage, publishedMessage?.Topic, "Published topic was incorrect");
            Assert.AreEqual(measurementData.Value, publishedMessage?.ConvertPayloadToString(), "Published payload was incorrect");
        }

        [TestMethod]
        public async Task ExecuteAsync_RepeatingReadError_StopService()
        {
            //Arrange
            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 0,
                StopOnRepeatedErrors = true
            };

            _mqttClientMock.Setup(m => m.IsConnected).Returns(true);
            _configMock.Setup(c => c.Value).Returns(config);
            _deviceScannerMock.Setup(d => d.GetDeviceDataAsync(It.IsAny<IList<SensorDevice>>())).Throws(new Exception());

            //Act      
            var btGwService = _serviceProvider.GetService<CollectorService>();
            var cts = new CancellationTokenSource();
#pragma warning disable CS4014
            // Run as fire & forget            
            Task.Run(() => btGwService.StartAsync(cts.Token));
#pragma warning restore

            await Task.Delay(500);
            cts.Cancel();

            _deviceScannerMock.Verify(d => d.GetDeviceDataAsync(It.IsAny<IList<SensorDevice>>()),
                Times.Exactly(CollectorService.MaxRepeatedReadFailCount), "The service did not stop after specified amount of failed reads");
        }

        [TestMethod]
        public async Task ExecuteAsync_PublishSensorDeviceTypeData_CorrectlyPublished()
        {
            var publishedMessages = new List<MqttApplicationMessage>();
            var sensorDevice = new SensorDevice()
            {
                Id = "Device1",
                SensorDeviceTypeId = "DeviceType1"
            };

            //Arrange
            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 1,
                SensorDevices = new List<SensorDevice>() { sensorDevice },
                SensorDeviceTypes = new List<SensorDeviceType>()
            };

            _mqttClientMock.Setup(m => m.IsConnected).Returns(true);
            _mqttClientMock.Setup(m => m.PublishAsync(It.IsAny<MqttApplicationMessage>(), CancellationToken.None)).Callback((MqttApplicationMessage message, CancellationToken ct) => { publishedMessages.Add(message); });
            _configMock.Setup(c => c.Value).Returns(config);

            //Act      
            var btGwService = _serviceProvider.GetService<CollectorService>();
            var cts = new CancellationTokenSource();

#pragma warning disable CS4014
            // Run as fire & forget
            Task.Run(() => btGwService.StartAsync(cts.Token).ConfigureAwait(false));
#pragma warning restore CS4014

            await Task.Delay(300);
            await btGwService.HandleConnectedAsync(new MqttClientConnectedEventArgs(new MqttClientAuthenticateResult()));
            cts.Cancel();

            // Assert
            var topic = MqttMessageHelper.GetSensorDeviceTopic(sensorDevice.Id);
            var payload = MqttMessageHelper.SerializePayload(sensorDevice);
            var message = publishedMessages.FirstOrDefault(m => m.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase));

            Assert.IsNotNull(message, "MQTT message for type data of sensor device was not received");
            Assert.AreEqual(topic, message?.Topic, "Sensor device message topic was not correct");
            Assert.AreEqual(payload, message?.ConvertPayloadToString(), "Sensor device message payload was not correct");
        }

        [TestMethod]
        public async Task ExecuteAsync_PublishSensorDeviceTypeTypeData_CorrectlyPublished()
        {
            var publishedMessages = new List<MqttApplicationMessage>();
            var sensorDeviceType = new SensorDeviceType()
            {
                Id = "DeviceType1",
                Sensors = new List<Sensor>() {
                        new Sensor(){
                            Id = "Sensor1",
                        }
                    }
            };

            //Arrange
            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 1,
                SensorDevices = new List<SensorDevice>(),
                SensorDeviceTypes = new List<SensorDeviceType>() { sensorDeviceType }
            };

            _mqttClientMock.Setup(m => m.IsConnected).Returns(true);
            _mqttClientMock.Setup(m => m.PublishAsync(It.IsAny<MqttApplicationMessage>(), CancellationToken.None)).Callback((MqttApplicationMessage message, CancellationToken ct) => { publishedMessages.Add(message); });
            _configMock.Setup(c => c.Value).Returns(config);

            //Act      
            var btGwService = _serviceProvider.GetService<CollectorService>();
            var cts = new CancellationTokenSource();

#pragma warning disable CS4014
            // Run as fire & forget
            Task.Run(() => btGwService.StartAsync(cts.Token).ConfigureAwait(false));
#pragma warning restore CS4014

            await Task.Delay(300);
            await btGwService.HandleConnectedAsync(new MqttClientConnectedEventArgs(new MqttClientAuthenticateResult()));
            cts.Cancel();

            // Assert
            var topic = MqttMessageHelper.GetSensorDeviceTypeTopic(sensorDeviceType.Id);
            var payload = MqttMessageHelper.SerializePayload(sensorDeviceType);
            var message = publishedMessages.FirstOrDefault(m => m.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase));

            Assert.IsNotNull(message, "MQTT message for type data of sensor device type was not received");
            Assert.AreEqual(topic, message?.Topic, "Sensor device type message topic was not correct");
            Assert.AreEqual(payload, message?.ConvertPayloadToString(), "Sensor device type message payload was not correct");
        }
    }
}
