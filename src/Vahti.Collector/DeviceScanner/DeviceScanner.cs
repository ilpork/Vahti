using BleReaderNet.Reader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Vahti.Collector.Configuration;
using Vahti.Collector.DeviceDataReader;
using Vahti.Shared.Data;
using Vahti.Shared.TypeData;
using Vahti.Shared.Utils;

namespace Vahti.Collector.DeviceScanner
{
    /// <summary>
    /// Provides functionality to scan devices and handle data as generic <see cref="MeasurementData"/>
    /// </summary>
    public class DeviceScanner : IDeviceScanner
    {
        public const int DeviceScanDuration = 10;

        private readonly IBleReader _bleReader;
        private readonly IDeviceDataReader _dataReader;
        private readonly ILogger<DeviceScanner> _logger;
        private readonly CollectorConfiguration _config;
        private readonly Dictionary<string, DateTime> _wasLastList = new Dictionary<string, DateTime>();
        private readonly ReadOnlyCollection<string> _bluetoothDevices = new List<string>() { Type.SensorDeviceTypeId.RuuviTag }.AsReadOnly();
        

        public DeviceScanner(ILogger<DeviceScanner> logger, IBleReader bleReader, IDeviceDataReader dataReader, IOptions<CollectorConfiguration> config)
        {
            _bleReader = bleReader;
            _dataReader = dataReader;
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

                measurementsList.AddRange(await _dataReader.ReadDeviceData(sensorDevice));

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
