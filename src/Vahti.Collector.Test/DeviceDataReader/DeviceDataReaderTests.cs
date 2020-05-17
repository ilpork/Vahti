using BleReaderNet.Device;
using BleReaderNet.Reader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Vahti.Shared.TypeData;

namespace Vahti.Collector.Test.DeviceScanner
{
    /// <Summary>
    /// Unit tests for <see cref="DeviceDataReader"/>
    /// </Summary>
    [TestClass]
    public class DeviceDataReaderTests
    {
        private Mock<ILogger<DeviceDataReader.DeviceDataReader>> _loggerMock;        
        private Mock<IBleReader> _bleReaderMock;        
        private ServiceProvider _serviceProvider;

        [TestInitialize]
        public void InitializeTest()
        {
            _loggerMock = new Mock<ILogger<DeviceDataReader.DeviceDataReader>>();            
            _bleReaderMock = new Mock<IBleReader>();
            
            var services = new ServiceCollection();
            services.AddSingleton<DeviceDataReader.DeviceDataReader>();
            services.AddSingleton(l => _loggerMock.Object);            
            services.AddSingleton(d => _bleReaderMock.Object);

            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Tests that a warning is logged if data can't be read from RuuviTag 
        /// </summary>        
        [TestMethod]        
        public async Task ReadDeviceDataAsync_RuuviTagNotFound_LogAWarning()
        {
            // Arrange
            const string ruuviTagAddress = "testAddress";
            var sensorDevice = new SensorDevice
            {
                SensorDeviceTypeId = Type.SensorDeviceTypeId.RuuviTag,
                Id = "SensorDevice1",
                Address = ruuviTagAddress
            };

            _bleReaderMock.Setup(b => b.GetManufacturerDataAsync<RuuviTag>(ruuviTagAddress)).ReturnsAsync(default(RuuviTag));
            var deviceDataReader = _serviceProvider.GetService<DeviceDataReader.DeviceDataReader>();

            // Act      
            var measurementList = await deviceDataReader.ReadDeviceDataAsync(sensorDevice);

            // Assert
            Assert.AreEqual(0, measurementList.Count, "Measurement list cound should be zero");
            _loggerMock.Verify(l => l.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), 
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once, "It should have been logged that RuuviTag could not be read");            
        }

        /// <summary>
        /// Tests that a warning is logged if data can't be read from RuuviTag 
        /// </summary>        
        [TestMethod]
        public async Task ReadDeviceDataAsync_RuuviTagFound_ReturnData()
        {
            // Arrange
            var numberFormatInfo = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            const string ruuviTagAddress = "testAddress";
            var sensorDevice = new SensorDevice
            {
                SensorDeviceTypeId = Type.SensorDeviceTypeId.RuuviTag,
                Id = "SensorDevice1",
                Address = ruuviTagAddress
            };
            var ruuviTag = new RuuviTag()
            {
                AccelerationX = 0.123,
                AccelerationY = 0.564,
                AccelerationZ = -0.153,
                AirPressure = 1060,
                BatteryVoltage = 2.11,
                DataFormat = 5,
                Humidity = 77,
                MacAddress = ruuviTagAddress,
                MeasurementSequenceNumber = 22,
                MovementCounter = 123,
                Temperature = 33.3,
                TxPower = 3
            };

            _bleReaderMock.Setup(b => b.GetManufacturerDataAsync<RuuviTag>(ruuviTagAddress)).ReturnsAsync(ruuviTag);
            var deviceDataReader = _serviceProvider.GetService<DeviceDataReader.DeviceDataReader>();

            // Act      
            var measurementList = await deviceDataReader.ReadDeviceDataAsync(sensorDevice);

            // Assert
            Assert.AreEqual(8, measurementList.Count, "Amount of returned measurements is not correct");
            Assert.AreEqual(ruuviTag.AccelerationX.Value.ToString(numberFormatInfo), measurementList.First(m => 
                m.SensorId.Equals("AccelerationX", StringComparison.OrdinalIgnoreCase)).Value, "Value of AccelerationX is not correct");
            Assert.AreEqual(ruuviTag.AccelerationY.Value.ToString(numberFormatInfo), measurementList.First(m => 
                m.SensorId.Equals("AccelerationY", StringComparison.OrdinalIgnoreCase)).Value, "Value of AccelerationY is not correct");
            Assert.AreEqual(ruuviTag.AccelerationZ.Value.ToString(numberFormatInfo), measurementList.First(m => 
                m.SensorId.Equals("AccelerationZ", StringComparison.OrdinalIgnoreCase)).Value, "Value of AccelerationZ is not correct");
            Assert.AreEqual(ruuviTag.AirPressure.Value.ToString(numberFormatInfo), measurementList.First(m => 
                m.SensorId.Equals("Pressure", StringComparison.OrdinalIgnoreCase)).Value, "Value of Pressure is not correct");
            Assert.AreEqual(ruuviTag.BatteryVoltage.Value.ToString(numberFormatInfo), measurementList.First(m => 
                m.SensorId.Equals("BatteryVoltage", StringComparison.OrdinalIgnoreCase)).Value, "Value of BatteryVoltage is not correct");
            Assert.AreEqual(ruuviTag.Humidity.Value.ToString(numberFormatInfo), measurementList.First(m => 
                m.SensorId.Equals("Humidity", StringComparison.OrdinalIgnoreCase)).Value, "Value of Humidity is not correct");
            Assert.AreEqual(ruuviTag.Temperature.Value.ToString(numberFormatInfo), measurementList.First(m => 
                m.SensorId.Equals("Temperature", StringComparison.OrdinalIgnoreCase)).Value, "Value of Temperature is not correct");
            Assert.AreEqual(ruuviTag.MovementCounter.Value.ToString(numberFormatInfo), measurementList.First(m => 
                m.SensorId.Equals("MovementCounter", StringComparison.OrdinalIgnoreCase)).Value, "Value of MovementCounter is not correct");
        }

        /// <summary>
        /// Tests that an error is raised if trying to read unsupported device
        /// </summary>        
        [TestMethod]
        public async Task ReadDeviceDataAsync_DeviceTypeUnknown_LogAnError()
        {
            // Arrange            
            var sensorDevice = new SensorDevice
            {
                SensorDeviceTypeId = "SomeUnsupportedDeviceType"                
            };
            
            var deviceDataReader = _serviceProvider.GetService<DeviceDataReader.DeviceDataReader>();

            // Act      
            var measurementList = await deviceDataReader.ReadDeviceDataAsync(sensorDevice);

            // Assert
            Assert.AreEqual(0, measurementList.Count, "Measurement list cound should be zero");
            _loggerMock.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once, "It should have been logged that the device type is unknown");
        }
    }
}
