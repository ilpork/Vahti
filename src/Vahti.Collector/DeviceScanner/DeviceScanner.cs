using BleReaderNet.Device;
using BleReaderNet.Reader;
using Iot.Device.DHTxx;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Gpio;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Vahti.Collector.Configuration;
using Vahti.Shared.Data;
using Vahti.Shared.TypeData;
using Vahti.Shared.Utils;

namespace Vahti.Collector.DeviceScanner
{
    /// <summary>
    /// Provides functionality to scan and read data from devices
    /// </summary>
    public class DeviceScanner : IDeviceScanner
    {
        public const int DeviceScanDuration = 10;

        private readonly IBleReader _bleReader;
        private readonly ILogger<DeviceScanner> _logger;
        private readonly CollectorConfiguration _config;
        private readonly Dictionary<string, DateTime> _wasLastList = new Dictionary<string, DateTime>();
        private readonly ReadOnlyCollection<string> _bluetoothDevices = new List<string>() { Type.SensorDeviceTypeId.RuuviTag }.AsReadOnly();
        private readonly NumberFormatInfo _numberFormatInfo = new NumberFormatInfo() { NumberDecimalSeparator = "." };

        public DeviceScanner(ILogger<DeviceScanner> logger, IBleReader bleReader, IOptions<CollectorConfiguration> config)
        {
            _bleReader = bleReader;
            _logger = logger;
            _config = config.Value;
        }

        public async Task<IList<MeasurementData>> GetDeviceDataAsync(IList<SensorDevice> sensorDevices)
        {
            if (sensorDevices == null)
            {
                return null;
            }
            
            var measurementsList = new List<MeasurementData>();            

            bool bluetoothDevicesScanned = false;
            foreach (var sensorDevice in sensorDevices)
            {
                // Scan devices only once per collecting session
                if (!bluetoothDevicesScanned && _bluetoothDevices.Contains(sensorDevice.SensorDeviceTypeId, StringComparer.OrdinalIgnoreCase))
                {
                    _logger.LogDebug($"{DateTime.Now}: Scanning for {DeviceScanDuration} seconds...");
                    await _bleReader.ScanAsync(_config.BluetoothAdapterName);
                    bluetoothDevicesScanned = true;
                }

                switch (sensorDevice.SensorDeviceTypeId)
                {
                    case Type.SensorDeviceTypeId.RuuviTag:
                        measurementsList.AddRange(await GetRuuviTagMeasurements(sensorDevice));
                        break;
                    case Type.SensorDeviceTypeId.Dht22:
                        measurementsList.AddRange(GetDht22Measurements(sensorDevice));
                        break;
                    // Add handlers for other device types here                
                    default:
                        _logger.LogError($"Type of device at {sensorDevice.Address} is not supported. " +
                            "Please check that it's been correctly defined in 'sensordevicetype' section of the configuration file");
                        return null;
                }

                if (sensorDevice.CalculatedMeasurements != null)
                {
                    foreach (var customMeasurement in sensorDevice.CalculatedMeasurements)
                    {
                        measurementsList.Add(GetCustomMeasurementValue(sensorDevice, measurementsList, customMeasurement));
                    }
                }
            }            

            return measurementsList;
        }

        private async Task<IList<MeasurementData>> GetRuuviTagMeasurements(SensorDevice sensorDevice)
        {
            var measurements = new List<MeasurementData>();

            var ruuviData = await _bleReader.GetManufacturerDataAsync<RuuviTag>(sensorDevice.Address);

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

            return measurements;
        }

        /// <summary>
        /// Read values from DHT22 sensor. It seems that the Dht22 support in Iot.Device.Bindings does not work very well, 
        /// but this can be considered as example on how to easily add support for additional devices
        /// </summary>        
        private IList<MeasurementData> GetDht22Measurements(SensorDevice sensorDevice)
        {
            var measurements = new List<MeasurementData>();
                        
            var pinNumber = int.Parse(sensorDevice.Address);

            using (var dht22 = new Dht22(pinNumber, PinNumberingScheme.Logical))
            {
                if (dht22.IsLastReadSuccessful)
                {
                    measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "temperature", Value = dht22.Temperature.Celsius.ToString(_numberFormatInfo) });
                    measurements.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "humidity", Value = dht22.Temperature.Celsius.ToString(_numberFormatInfo) });
                }
                else
                {
                    _logger.LogWarning($"{DateTime.Now}: Could not read data from {sensorDevice.Id}");
                }
            }           

            return measurements;
        }

        private MeasurementData GetCustomMeasurementValue(SensorDevice sensorDevice, List<MeasurementData> measurements, CustomMeasurementRule rule)
        {
            var sensorMeasurement = measurements.FirstOrDefault(m => m.SensorId.Equals(rule.SensorId, StringComparison.OrdinalIgnoreCase));

            if (sensorMeasurement == null)
            {
                _logger.LogError($"No sensor '{rule.SensorId}' found from measurements of device type '{sensorDevice.SensorDeviceTypeId}'");
                return null;
            }

            var data = new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = rule.Id };
            var comparisonResult = LogicHelper.Compare(sensorMeasurement.Value, rule.Value, rule.Operator);

            switch (rule.Type)
            {
                case Shared.Enum.ValueType.Default:
                    data.Value = comparisonResult ? "true" : "false";
                    break;
                case Shared.Enum.ValueType.WasLast:
                    var keyName = $"{sensorDevice.Id}-{sensorMeasurement.SensorId}-{rule.Id}";
                    if (comparisonResult)
                    {
                        if (!_wasLastList.ContainsKey(keyName))
                        {
                            _wasLastList.Add(keyName, DateTime.Now);
                        }
                        else
                        {
                            _wasLastList[keyName] = DateTime.Now;
                        }
                        data.Value = "0";
                    }
                    else
                    {
                        if (_wasLastList.ContainsKey(keyName))
                        {
                            var ts = DateTime.Now - _wasLastList[keyName];
                            data.Value = ((int)ts.TotalSeconds).ToString();
                        }
                        else
                        {
                            data.Value = "-1";
                        }
                    }
                    break;
                default:
                    break;

            }
            return data;
        }
    }
}
