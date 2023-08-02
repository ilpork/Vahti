using BleReaderNet.Device;
using BleReaderNet.Reader;
using Iot.Device.DHTxx;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Globalization;
using System.Threading.Tasks;
using Vahti.Shared.Data;
using Vahti.Shared.TypeData;

namespace Vahti.Collector.DeviceDataReader
{
    /// <summary>
    /// Handle device specific data and returns it as <see cref="MeasurementData"/>
    /// </summary>
    public class DeviceDataReader : IDeviceDataReader
    {
        private readonly NumberFormatInfo _numberFormatInfo = new NumberFormatInfo() { NumberDecimalSeparator = "." };
        private readonly IBleReader _bleReader;        
        private readonly ILogger<DeviceDataReader> _logger;        
        
        public DeviceDataReader(ILogger<DeviceDataReader> logger, IBleReader bleReader)
        {
            _bleReader = bleReader;            
            _logger = logger;            
        }

        public async Task<IList<MeasurementData>> ReadDeviceDataAsync(SensorDevice sensorDevice)
        {
            var measurements = new List<MeasurementData>();

            switch (sensorDevice.SensorDeviceTypeId)
            {
                case Type.SensorDeviceTypeId.RuuviTag:
                    measurements.AddRange(await GetRuuviTagMeasurementsAsync(sensorDevice));
                    break;
                case Type.SensorDeviceTypeId.Dht22:
                    measurements.AddRange(GetDht22Measurements(sensorDevice));
                    break;
                // Add handlers for other device types here                
                default:
                    _logger.LogError($"Type of device at {sensorDevice.Address} is not supported. " +
                        "Please check that it's been correctly defined in 'sensordevicetype' section of the configuration file");
                    break;
            }
            return measurements;
        }

        private async Task<IList<MeasurementData>> GetRuuviTagMeasurementsAsync(SensorDevice sensorDevice)
        {
            var measurements = new List<MeasurementData>();

            var ruuviData = await _bleReader.GetManufacturerDataAsync<RuuviTag>(sensorDevice.Address);

            if (ruuviData == null)
            {
                _logger.LogWarning($"{DateTime.Now}: Could not read manufacturer data from '{sensorDevice.Id}' at '{sensorDevice.Address}'");                
            }
            else
            {
                measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "temperature", Value = ruuviData.Temperature.Value.ToString(_numberFormatInfo) });
                measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "humidity", Value = ruuviData.Humidity.Value.ToString(_numberFormatInfo) });
                measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "pressure", Value = ruuviData.AirPressure.Value.ToString(_numberFormatInfo) });

                if (ruuviData.AccelerationX != null)
                    measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "accelerationX", Value = ruuviData.AccelerationX.Value.ToString(_numberFormatInfo) });
                if (ruuviData.AccelerationY != null)
                    measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "accelerationY", Value = ruuviData.AccelerationY.Value.ToString(_numberFormatInfo) });
                if (ruuviData.AccelerationZ != null)
                    measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "accelerationZ", Value = ruuviData.AccelerationZ.Value.ToString(_numberFormatInfo) });
                if (ruuviData.BatteryVoltage != null)
                    measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "batteryVoltage", Value = ruuviData.BatteryVoltage.Value.ToString(_numberFormatInfo) });
                if (ruuviData.MovementCounter != null)
                    measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "movementCounter", Value = ruuviData.MovementCounter.Value.ToString(_numberFormatInfo) });
            }            

            return measurements;
        }

        /// <summary>
        /// Read values from DHT22 sensor. 
        /// Dht22 support in Iot.Device.Bindings did not work very well when I tried it in the past with old package version, but this code can be considered as example on how to easily add support for additional devices
        /// </summary>        
        private IList<MeasurementData> GetDht22Measurements(SensorDevice sensorDevice)
        {
            var measurements = new List<MeasurementData>();

            var pinNumber = int.Parse(sensorDevice.Address);

            using (var dht22 = new Dht22(pinNumber, PinNumberingScheme.Logical))
            {
                if (dht22.TryReadTemperature(out var temp) && dht22.TryReadHumidity(out var humidity))
                {
                    measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "temperature", Value = temp.DegreesCelsius.ToString(_numberFormatInfo) });
                    measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "humidity", Value = humidity.Percent.ToString(_numberFormatInfo) });
                }
                else
                {
                    _logger.LogWarning($"{DateTime.Now}: Could not read data from {sensorDevice.Id}");
                }
            }

            return measurements;
        }
    }
}
