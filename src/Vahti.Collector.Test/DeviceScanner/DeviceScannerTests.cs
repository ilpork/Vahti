using BleReaderNet.Reader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vahti.Collector.Configuration;
using Vahti.Collector.DeviceDataReader;
using Vahti.Shared.Data;
using Vahti.Shared.TypeData;

namespace Vahti.Collector.Test.DeviceScanner
{
    /// <Summary>
    /// Unit tests for <see cref="CollectorService"/>
    /// </Summary>
    [TestClass]
    public class DeviceScannerTests
    {
        private Mock<ILogger<Collector.DeviceScanner.DeviceScanner>> _loggerMock;
        private Mock<IOptions<CollectorConfiguration>> _configMock;
        private Mock<IBleReader> _bleReaderMock;
        private Mock<IDeviceDataReader> _dataReaderMock;
        private ServiceProvider _serviceProvider;

        [TestInitialize]
        public void InitializeTest()
        {
            _loggerMock = new Mock<ILogger<Collector.DeviceScanner.DeviceScanner>>();
            _configMock = new Mock<IOptions<CollectorConfiguration>>();
            _dataReaderMock = new Mock<IDeviceDataReader>();
            _bleReaderMock = new Mock<IBleReader>();
            
            var services = new ServiceCollection();
            services.AddSingleton<Collector.DeviceScanner.DeviceScanner>();
            services.AddSingleton(l => _loggerMock.Object);
            services.AddSingleton(c => _configMock.Object);
            services.AddSingleton(d => _dataReaderMock.Object);
            services.AddSingleton(d => _bleReaderMock.Object);

            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Tests the basic loop handling so that correct amount of measurements is returned for devices/measurements 
        /// and returned data is correct
        /// </summary>
        /// <param name="sensorDeviceCount">Amount of sensor devices</param>
        /// <param name="measurementCount">Amount of measurements</param>        
        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 0)]
        [DataRow(0, 1)]
        [DataRow(1, 1)]
        [DataRow(2, 1)]
        [DataRow(1, 2)]
        [DataRow(2, 2)]
        public async Task GetDeviceDataAsync_DataReturned_True(int sensorDeviceCount, int measurementCount)
        {
            var sensorDevices = new List<SensorDevice>();
            var sensorDeviceTypes = new List<SensorDeviceType>();
            var measurementList = new Dictionary<string, List<MeasurementData>>();

            // Arrange
            for (int i=1; i<=sensorDeviceCount; i++)
            {
                sensorDevices.Add(new SensorDevice()
                {
                    Id = $"Device{i}",
                    SensorDeviceTypeId = $"DeviceType{i}"
                });

                sensorDeviceTypes.Add(new SensorDeviceType()
                {
                    Id = $"DeviceType{i}",
                    Sensors = new List<Sensor>() {
                        new Sensor(){
                            Id = $"Sensor{i}",
                        }
                    }
                });
            }
            var measurementTimestampBaseTicks = 1434135153;
            var measurementValueBase = "123";

            foreach (var sensorDevice in sensorDevices)
            {
                var list = new List<MeasurementData>();

                for (int i = 1; i <= measurementCount; i++)
                {
                    list.Add(new MeasurementData()
                    {
                        SensorDeviceId = sensorDevice.Id,
                        SensorId = sensorDeviceTypes.First(s => s.Id.Equals(sensorDevice.SensorDeviceTypeId)).Sensors[0].Id,
                        Timestamp = new DateTime(measurementTimestampBaseTicks + i),
                        Value = $"{measurementValueBase}{i}"
                    });
                }
                measurementList.Add(sensorDevice.Id, list);
            }                

            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 0,
                SensorDevices = sensorDevices,
                SensorDeviceTypes = sensorDeviceTypes
            };        
            
            
            _configMock.Setup(c => c.Value).Returns(config);
            _dataReaderMock.Setup(d => d.ReadDeviceDataAsync(It.IsAny<SensorDevice>()))
                .ReturnsAsync((SensorDevice p) => measurementList[p.Id]);

            var deviceScanner = _serviceProvider.GetService<Collector.DeviceScanner.DeviceScanner>();

            //Act      
            var fullList = await deviceScanner.GetDeviceDataAsync(config.SensorDevices);

            // Assert
            Assert.AreEqual(sensorDeviceCount * measurementCount, fullList.Count, "Measurement count does not match");

            foreach (var sensorDevice in sensorDevices)
            {
                var list = new List<MeasurementData>();

                for (int i = 1; i <= measurementCount; i++)
                {
                    Assert.IsTrue(fullList.Any(m =>
                        m.SensorDeviceId.Equals(sensorDevice.Id) &&
                        m.SensorId.Equals(sensorDeviceTypes.First(s => s.Id.Equals(sensorDevice.SensorDeviceTypeId)).Sensors[0].Id) &&
                        m.Timestamp.Ticks == measurementTimestampBaseTicks + i &&
                        m.Value.Equals($"{measurementValueBase}{i}")
                        ));
                }                
            }
        }        

        /// <summary>
        /// Tests that Bluetooth scan is not made if no Bluetooth devices exist
        /// </summary>        
        [TestMethod]        
        public async Task GetDeviceDataAsync_NoBluetoothDevices_NoScanMade()
        {
            var sensorDeviceTypeId = "SomeSensorType";

            // Arrange
            var sensorDevice = new SensorDevice
            {
                SensorDeviceTypeId = sensorDeviceTypeId,
                Id = "SensorDevice1"
            };

            var sensorDeviceType = new SensorDeviceType()
            {
                Id = sensorDeviceTypeId,
                Sensors = new List<Sensor>() {
                        new Sensor(){
                            Id = "Sensor1",
                        }
                    }
            };
            _dataReaderMock.Setup(d => d.ReadDeviceDataAsync(It.IsAny<SensorDevice>()))
                .ReturnsAsync(new List<MeasurementData>());

            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 1,
                SensorDevices = new List<SensorDevice>() { sensorDevice },
                SensorDeviceTypes = new List<SensorDeviceType>() { sensorDeviceType }
            };

            _configMock.Setup(c => c.Value).Returns(config);
            
            var deviceScanner = _serviceProvider.GetService<Collector.DeviceScanner.DeviceScanner>();

            // Act      
            var fullList = await deviceScanner.GetDeviceDataAsync(config.SensorDevices);

            // Assert 
            _bleReaderMock.Verify(b => b.ScanAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never, 
                "Bluetooth scan should not be made when it's not needed");
        }

        /// <summary>
        /// Tests that Bluetooth scan is made if Bluetooth devices exist
        /// </summary>        
        [TestMethod]
        public async Task GetDeviceDataAsync_BluetoothDevices_ScanMade()
        {
            var sensorDeviceTypeId = "RuuviTag";

            // Arrange
            var sensorDevice = new SensorDevice
            {
                SensorDeviceTypeId = sensorDeviceTypeId,
                Id = "SensorDevice1"
            };

            var sensorDeviceType = new SensorDeviceType()
            {
                Id = sensorDeviceTypeId,
                Sensors = new List<Sensor>() {
                        new Sensor(){
                            Id = "Sensor1",
                        }
                    }
            };
            _dataReaderMock.Setup(d => d.ReadDeviceDataAsync(It.IsAny<SensorDevice>()))
                .ReturnsAsync(new List<MeasurementData>());

            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 1,
                SensorDevices = new List<SensorDevice>() { sensorDevice },
                SensorDeviceTypes = new List<SensorDeviceType>() { sensorDeviceType }
            };

            _configMock.Setup(c => c.Value).Returns(config);

            var deviceScanner = _serviceProvider.GetService<Collector.DeviceScanner.DeviceScanner>();

            // Act      
            var fullList = await deviceScanner.GetDeviceDataAsync(config.SensorDevices);

            // Assert 
            _bleReaderMock.Verify(b => b.ScanAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Once,
                "Bluetooth scan should've been made");
        }

        /// <summary>
        /// Tests that custom measurement data is not returned if target sensor is not found
        /// </summary>        
        [TestMethod]        
        public async Task GetDeviceDataAsync_CustomMeasurementSensorNotExists_ReturnNull()
        {
            var sensorDeviceTypeId = "SomeSensorType";
            var sensorDeviceId = "SomeSensorDevice";
            var sensorId = "SomeSensor";
            var customRule = new CustomMeasurementRule()
            {
                Class = "MyClass",
                Id = "MyId",
                Name = "My custom measurement",
                Unit = "qwerty",
                Operator = Shared.Enum.OperatorType.IsGreaterThan,
                SensorId = "UnknownSensor",
                Type = Shared.Enum.ValueType.Default,
                Value = "somevalue"
            };

            // Arrange
            var sensorDevice = new SensorDevice
            {
                SensorDeviceTypeId = sensorDeviceTypeId,
                Id = sensorDeviceId,
                CalculatedMeasurements = new List<CustomMeasurementRule>()
                {
                    customRule
                }
            };

            var sensorDeviceType = new SensorDeviceType()
            {
                Id = sensorDeviceTypeId,
                Sensors = new List<Sensor>() {
                        new Sensor(){
                            Id = sensorId,
                        }
                    }
            };
            _dataReaderMock.Setup(d => d.ReadDeviceDataAsync(It.IsAny<SensorDevice>()))
                .ReturnsAsync(new List<MeasurementData>() { new MeasurementData()
                {
                    SensorDeviceId = sensorDeviceId,
                    SensorId = sensorId,
                    Value = "some value"
                }});

            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 1,
                SensorDevices = new List<SensorDevice>() { sensorDevice },
                SensorDeviceTypes = new List<SensorDeviceType>() { sensorDeviceType }
            };

            _configMock.Setup(c => c.Value).Returns(config);

            var deviceScanner = _serviceProvider.GetService<Collector.DeviceScanner.DeviceScanner>();

            // Act      
            var fullList = await deviceScanner.GetDeviceDataAsync(config.SensorDevices);

            // Assert 
            Assert.AreEqual(1, fullList.Count, "Amount of measurements is not correct");
            Assert.AreEqual(sensorDeviceId, fullList[0].SensorDeviceId, "Sensor device ID is not correct");
        }

        /// <summary>
        /// Tests that custom measurement data is returned correctly for with default comparison type
        /// </summary>        
        [TestMethod]
        [DataRow(Shared.Enum.OperatorType.IsGreaterThan, "1.23", "0.66", true)]
        [DataRow(Shared.Enum.OperatorType.IsGreaterThan, "0.44", "0.66", false)]
        [DataRow(Shared.Enum.OperatorType.IsLessThan, "1.23", "0.66", false)]
        [DataRow(Shared.Enum.OperatorType.IsLessThan, "1.23", "1.88", true)]
        [DataRow(Shared.Enum.OperatorType.IsEqualTo, "66", "66", true)]
        [DataRow(Shared.Enum.OperatorType.IsEqualTo, "66", "68", false)]
        public async Task GetDeviceDataAsync_DefaultTypeCustomMeasurementExists_Returned(Shared.Enum.OperatorType operatorType,
            string measurementValue, string customRuleValue, bool expectedValue)
        {
            var sensorDeviceTypeId = "SomeSensorType";
            var sensorDeviceId = "SomeSensorDevice";
            var sensorId = "SomeSensor";            
            var customRule = new CustomMeasurementRule()
            {
                Class = "MyClass",
                Id = "MyId",
                Name = "My custom measurement",
                Unit = "qwerty",
                Operator = operatorType,
                SensorId = sensorId,
                Type = Shared.Enum.ValueType.Default,
                Value = customRuleValue
            };

            // Arrange
            var sensorDevice = new SensorDevice
            {
                SensorDeviceTypeId = sensorDeviceTypeId,
                Id = sensorDeviceId,
                CalculatedMeasurements = new List<CustomMeasurementRule>()
                {
                    customRule
                }
            };

            var sensorDeviceType = new SensorDeviceType()
            {
                Id = sensorDeviceTypeId,
                Sensors = new List<Sensor>() {
                        new Sensor(){
                            Id = sensorId,
                        }
                    }
            };
            _dataReaderMock.Setup(d => d.ReadDeviceDataAsync(It.IsAny<SensorDevice>()))
                .ReturnsAsync(new List<MeasurementData>() { new MeasurementData()
                {
                    SensorDeviceId = sensorDeviceId,
                    SensorId = sensorId,
                    Value = measurementValue
                }});

            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 1,
                SensorDevices = new List<SensorDevice>() { sensorDevice },
                SensorDeviceTypes = new List<SensorDeviceType>() { sensorDeviceType }
            };

            _configMock.Setup(c => c.Value).Returns(config);

            var deviceScanner = _serviceProvider.GetService<Collector.DeviceScanner.DeviceScanner>();

            // Act      
            var fullList = await deviceScanner.GetDeviceDataAsync(config.SensorDevices);

            // Assert 
            Assert.AreEqual(2, fullList.Count, "Amount of measurements is not correct");

            var customMeasurement = fullList.First(f => f.SensorId.Equals(customRule.Id, StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual(sensorDeviceId, customMeasurement.SensorDeviceId, "Sensor device ID does not match");
            Assert.AreNotEqual(default(DateTime), customMeasurement.Timestamp, "Timestamp has not been set");
            Assert.AreEqual(expectedValue, bool.Parse(customMeasurement.Value), "Value is not what expected");
        }

        /// <summary>
        /// Tests that custom measurement data is returned correctly for with ´WasLast' comparison type
        /// </summary>        
        [TestMethod]
        [DataRow(Shared.Enum.OperatorType.IsGreaterThan, "1.23", "0.66", "0")]
        [DataRow(Shared.Enum.OperatorType.IsGreaterThan, "0.44", "0.66", "-1")]
        [DataRow(Shared.Enum.OperatorType.IsLessThan, "0.33", "0.66", "0")]
        [DataRow(Shared.Enum.OperatorType.IsLessThan, "1.1", "0.66", "-1")]
        [DataRow(Shared.Enum.OperatorType.IsEqualTo, "4", "4", "0")]
        [DataRow(Shared.Enum.OperatorType.IsEqualTo, "5", "8", "-1")]
        public async Task GetDeviceDataAsync_WasLastTypeCustomMeasurementExists_HandleValueZeroAndMinusOne(
            Shared.Enum.OperatorType operatorType, string measurementValue, string customRuleValue, string expectedComparisonValue)
        {
            var sensorDeviceTypeId = "SomeSensorType";
            var sensorDeviceId = "SomeSensorDevice";
            var sensorId = "SomeSensor";
            var customRule = new CustomMeasurementRule()
            {
                Class = "MyClass",
                Id = "MyId",
                Name = "My custom measurement",
                Unit = "qwerty",
                Operator = operatorType,
                SensorId = sensorId,
                Type = Shared.Enum.ValueType.WasLast,
                Value = customRuleValue
            };

            // Arrange
            var sensorDevice = new SensorDevice
            {
                SensorDeviceTypeId = sensorDeviceTypeId,
                Id = sensorDeviceId,
                CalculatedMeasurements = new List<CustomMeasurementRule>()
                {
                    customRule
                }
            };

            var sensorDeviceType = new SensorDeviceType()
            {
                Id = sensorDeviceTypeId,
                Sensors = new List<Sensor>() {
                        new Sensor(){
                            Id = sensorId,
                        }
                    }
            };
            _dataReaderMock.Setup(d => d.ReadDeviceDataAsync(It.IsAny<SensorDevice>()))               
                .ReturnsAsync(new List<MeasurementData>() { new MeasurementData()
                {
                    SensorDeviceId = sensorDeviceId,
                    SensorId = sensorId,
                    Value = measurementValue
                }})
                ;

            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 1,
                SensorDevices = new List<SensorDevice>() { sensorDevice },
                SensorDeviceTypes = new List<SensorDeviceType>() { sensorDeviceType }
            };

            _configMock.Setup(c => c.Value).Returns(config);

            var deviceScanner = _serviceProvider.GetService<Collector.DeviceScanner.DeviceScanner>();

            // Act      
            var fullList = await deviceScanner.GetDeviceDataAsync(config.SensorDevices);

            // Assert 
            Assert.AreEqual(2, fullList.Count, "Amount of measurements is not correct");
            
            var customMeasurement = fullList.First(f => f.SensorId.Equals(customRule.Id, StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual(sensorDeviceId, customMeasurement.SensorDeviceId, "Sensor device ID does not match");
            Assert.AreNotEqual(default(DateTime), customMeasurement.Timestamp, "Timestamp has not been set");
            Assert.AreEqual(expectedComparisonValue, customMeasurement.Value, "Value is not what expected");
        }

        /// <summary>
        /// Tests that custom measurement data is returned correctly for with 'WasLast' type with sequence:
        /// Condition false -> condition true -> condition false -> condition true
        /// </summary>        
        [TestMethod]        
        public async Task GetDeviceDataAsync_WasLastTypeCustomMeasurement_SequenceWorksCorrectly()
        {
            var sensorDeviceTypeId = "SomeSensorType";
            var sensorDeviceId = "SomeSensorDevice";
            var sensorId = "SomeSensor";
            var customRule = new CustomMeasurementRule()
            {
                Class = "MyClass",
                Id = "MyId",
                Name = "My custom measurement",
                Unit = "qwerty",
                Operator = Shared.Enum.OperatorType.IsGreaterThan,
                SensorId = sensorId,
                Type = Shared.Enum.ValueType.WasLast,
                Value = "1.66"
            };

            // Arrange
            var sensorDevice = new SensorDevice
            {
                SensorDeviceTypeId = sensorDeviceTypeId,
                Id = sensorDeviceId,
                CalculatedMeasurements = new List<CustomMeasurementRule>()
                {
                    customRule
                }
            };

            var sensorDeviceType = new SensorDeviceType()
            {
                Id = sensorDeviceTypeId,
                Sensors = new List<Sensor>() {
                        new Sensor(){
                            Id = sensorId,
                        }
                    }
            };
            _dataReaderMock.SetupSequence(d => d.ReadDeviceDataAsync(It.IsAny<SensorDevice>()))
                .ReturnsAsync(new List<MeasurementData>() { new MeasurementData()
                {
                    SensorDeviceId = sensorDeviceId,
                    SensorId = sensorId,
                    Value = "0.1"
                }})
                .ReturnsAsync(new List<MeasurementData>() { new MeasurementData()
                {
                    SensorDeviceId = sensorDeviceId,
                    SensorId = sensorId,
                    Value = "2.1"
                }})
                .ReturnsAsync(new List<MeasurementData>() { new MeasurementData()
                {
                    SensorDeviceId = sensorDeviceId,
                    SensorId = sensorId,
                    Value = "0.1"
                }})
                .ReturnsAsync(new List<MeasurementData>() { new MeasurementData()
                {
                    SensorDeviceId = sensorDeviceId,
                    SensorId = sensorId,
                    Value = "2.1"
                }})
                ;

            var config = new CollectorConfiguration
            {
                CollectorEnabled = true,
                BluetoothAdapterName = "test",
                ScanIntervalSeconds = 1,
                SensorDevices = new List<SensorDevice>() { sensorDevice },
                SensorDeviceTypes = new List<SensorDeviceType>() { sensorDeviceType }
            };

            _configMock.Setup(c => c.Value).Returns(config);

            var deviceScanner = _serviceProvider.GetService<Collector.DeviceScanner.DeviceScanner>();

            // Act      
            var fullList = await deviceScanner.GetDeviceDataAsync(config.SensorDevices); 
            var customMeasurement = fullList.First(f => f.SensorId.Equals(customRule.Id, StringComparison.OrdinalIgnoreCase));            
            Assert.AreEqual("-1", customMeasurement.Value, "Value is not what expected when condition has not been true yet");

            fullList = await deviceScanner.GetDeviceDataAsync(config.SensorDevices);
            customMeasurement = fullList.First(f => f.SensorId.Equals(customRule.Id, StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual("0", customMeasurement.Value, "Value is not what expected when condition became true");

            // Wait more than one second to get "WasLast" value to get updated to 1
            await Task.Delay(1200);

            fullList = await deviceScanner.GetDeviceDataAsync(config.SensorDevices);
            customMeasurement = fullList.First(f => f.SensorId.Equals(customRule.Id, StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual("1", customMeasurement.Value, "Value is not what expected when condition became false again");

            fullList = await deviceScanner.GetDeviceDataAsync(config.SensorDevices);
            customMeasurement = fullList.First(f => f.SensorId.Equals(customRule.Id, StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual("0", customMeasurement.Value, "Value is not what expected when condition is true again");
        }
    }
}
