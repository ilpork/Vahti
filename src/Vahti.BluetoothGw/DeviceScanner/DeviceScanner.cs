using BleReaderNet.Device;
using BleReaderNet.Reader;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vahti.Shared.Data;
using Vahti.Shared.TypeData;
using Vahti.Shared.Utils;

namespace Vahti.BluetoothGw.DeviceScanner
{
    /// <summary>
    /// Provides functionality to scan and read data from devices
    /// </summary>
    public class DeviceScanner : IDeviceScanner
    {
        private readonly IBleReader _bleReader;
        private readonly ILogger<DeviceScanner> _logger;
        private readonly Dictionary<string, DateTime> _wasLastList = new Dictionary<string, DateTime>();

        public DeviceScanner(ILogger<DeviceScanner> logger, IBleReader bleReader)
        {
            _bleReader = bleReader;
            _logger = logger;
        }
        public async Task ScanDevicesAsync(string adapterName, int scanDurationSeconds)
        {
            await _bleReader.ScanAsync(adapterName, scanDurationSeconds);
        }

        public async Task<List<MeasurementData>> GetDeviceDataAsync(SensorDevice sensorDevice)
        {
            if (sensorDevice == null)
            {
                return null;
            }

            var measurementsList = new List<MeasurementData>();
            var nfi = new NumberFormatInfo() { NumberDecimalSeparator = "." };

            switch (sensorDevice.SensorDeviceTypeId)
            {
                case "RuuviTag":
                    var ruuviData = await _bleReader.GetManufacturerDataAsync<RuuviTag>(sensorDevice.Address);

                    measurementsList.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "temperature", Value = ruuviData.Temperature.Value.ToString(nfi) });
                    measurementsList.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "humidity", Value = ruuviData.Humidity.Value.ToString(nfi) });
                    measurementsList.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "pressure", Value = ruuviData.AirPressure.Value.ToString(nfi) });

                    if (ruuviData.AccelerationX != null)
                        measurementsList.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "accelerationX", Value = ruuviData.AccelerationX.Value.ToString(nfi) });
                    if (ruuviData.AccelerationY != null)
                        measurementsList.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "accelerationY", Value = ruuviData.AccelerationY.Value.ToString(nfi) });
                    if (ruuviData.AccelerationZ != null)
                        measurementsList.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "accelerationZ", Value = ruuviData.AccelerationZ.Value.ToString(nfi) });
                    if (ruuviData.BatteryVoltage != null)
                        measurementsList.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "batteryVoltage", Value = ruuviData.BatteryVoltage.Value.ToString(nfi) });
                    if (ruuviData.MovementCounter != null)
                        measurementsList.Add(new MeasurementData() { SensorDeviceId = sensorDevice.Id, SensorId = "movementCounter", Value = ruuviData.MovementCounter.Value.ToString(nfi) });

                    // Publish measurement sequence number, tx power etc here if needed
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

            return measurementsList;
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
