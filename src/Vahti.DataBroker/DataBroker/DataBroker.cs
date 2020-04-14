using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Vahti.DataBroker.AlertHandler;
using Vahti.DataBroker.Configuration;
using Vahti.DataBroker.DataProvider;
using Vahti.Shared;
using Vahti.Shared.Data;
using Vahti.Shared.DataProvider;
using Vahti.Shared.TypeData;

namespace Vahti.DataBroker.DataBroker
{
    /// <summary>
    /// Handles incoming MQTT messages and relays the data to cloud database
    /// </summary>
    public class DataBroker : IDataBroker 
    {        
        private readonly ILogger<DataBroker> _logger;
        private readonly DataBrokerConfiguration _config;
        private readonly IDataProvider _dataProvider;
        private readonly IHistoryDataProvider _historyDataProvider;
        private readonly IAlertHandler _alertHandler;

        private readonly List<LocationData> _locations = new List<LocationData>();
        private readonly List<SensorDeviceType> _sensorDeviceTypes = new List<SensorDeviceType>();
        private readonly List<SensorDevice> _sensorDevices = new List<SensorDevice>();
        private bool _sensorDeviceTypesChanged;
        private bool _sensorDevicesChanged;
        private bool _locationChanged;

        public DataBroker(ILogger<DataBroker> logger, IOptions<DataBrokerConfiguration> dataBrokerConfig, IDataProvider dataProvider,
            IHistoryDataProvider historyDataProvider, IAlertHandler alertHandler)
        {
            _logger = logger;
            _config = dataBrokerConfig.Value;
            _dataProvider = dataProvider;
            _historyDataProvider = historyDataProvider;
            _alertHandler = alertHandler;
        }

        private bool CloudPublishEnabled => _config.CloudPublishConfiguration != null && _config.CloudPublishConfiguration.Enabled;

        public async Task<bool> PublishCurrentData()
        {
            if (!CloudPublishEnabled)
            {
                return false;                
            }

            if (_sensorDeviceTypesChanged)
            {
                foreach (var type in _sensorDeviceTypes)
                {
                    await _dataProvider.StoreNewItemAsync(type);
                }
                _sensorDeviceTypesChanged = false;
            }
            if (_sensorDevicesChanged)
            {
                foreach (var device in _sensorDevices)
                {
                    await _dataProvider.StoreNewItemAsync(device);
                }
                _sensorDeviceTypesChanged = false;
            }
            if (_locationChanged)
            {
                _logger.LogInformation($"{DateTime.Now}: Published data for {_locations.Count} locations");
                foreach (var location in _locations)
                {
                    await _dataProvider.StoreNewItemAsync(location);
                    await _historyDataProvider.AddItems(location.Measurements);
                }
                _sensorDeviceTypesChanged = false;
            }
            return true;
        }

        public async Task<bool> SendAlerts()
        {
            return await _alertHandler.Send(_locations);
        }

        public async Task<bool> PublishHistoryData()
        {
            if (!CloudPublishEnabled)
            {
                return false;
            }

            var historyData = new HistoryData() { Id = "1", DataList = new List<MeasurementHistoryData>() };

            foreach (var sensorDevice in _sensorDevices)
            {
                foreach (var sensor in _sensorDeviceTypes.First(t => t.Id.Equals(sensorDevice.SensorDeviceTypeId, StringComparison.OrdinalIgnoreCase)).Sensors)
                {
                    var measurementHistory = new MeasurementHistoryData() { SensorDeviceId = sensorDevice.Id, SensorId = sensor.Id };
                    var allMeasurements = await _historyDataProvider.GetHistory(sensorDevice.Id, sensor.Id, _config.CloudPublishConfiguration.HistoryLengthDays);

                    var groups = from s in allMeasurements
                                 let groupKey = new DateTime(s.Timestamp.Year, s.Timestamp.Month, s.Timestamp.Day, s.Timestamp.Hour, 0, 0)
                                 group s by groupKey into g
                                 select new HistoryValueData
                                 {
                                     Timestamp = g.Key,
                                     Value = g.First().Value
                                 };
                    measurementHistory.Values = groups.ToList();
                    historyData.DataList.Add(measurementHistory);
                }
            }

            if (historyData.DataList.Count > 0)
            {
                await _dataProvider.StoreNewItemAsync(historyData);
                _logger.LogInformation($"{DateTime.Now}: Published history data for {historyData.DataList.Count} sensors");
            }

            return true;
        }

        public async Task<bool> DeleteOldHistoryData()
        {
            if (!CloudPublishEnabled)
            {
                return false;
            }

            await _historyDataProvider.DeleteOldHistory(_config.CloudPublishConfiguration.HistoryLengthDays);
            return true;
        }

        public void MqttMessageReceived(string topic, string payload)
        {
            var splittedTopic = topic.Split('/');

            switch (splittedTopic[0])
            {
                case Constant.TopicMeasurement:
                    var sensorId = splittedTopic[^1];
                    var sensorDeviceId = splittedTopic[^2];
                    var locationName = string.Join('/', splittedTopic, 1, splittedTopic.Length - 3);

                    var location = _locations.FirstOrDefault(l => l.Name.Equals(locationName));
                    if (location == null)
                    {
                        _locations.Add(location = new LocationData() { Name = locationName, Id = locationName });
                    }
                    location.Timestamp = DateTime.Now;
                    location.UpdateInterval = _config.CloudPublishConfiguration.UpdateIntervalMinutes;

                    var existingMeasurement = location.Measurements.FirstOrDefault(m => topic.EndsWith($"{m.SensorDeviceId}/{m.SensorId}"));
                    if (existingMeasurement != null)
                    {
                        existingMeasurement.Value = payload;
                        existingMeasurement.Timestamp = DateTime.Now;
                    }
                    else
                    {
                        location.Measurements.Add(new MeasurementData() { Timestamp = DateTime.Now, SensorDeviceId = sensorDeviceId, SensorId = sensorId, Value = payload });
                    }
                    _locationChanged = true;
                    break;
                case Constant.TopicSensorDeviceType:
                    var sensorDeviceType = JsonSerializer.Deserialize<SensorDeviceType>(payload, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    var existingType = _sensorDeviceTypes.FirstOrDefault(s => s.Id.Equals(sensorDeviceType.Id, StringComparison.OrdinalIgnoreCase));
                    if (existingType == null)
                    {
                        _sensorDeviceTypes.Add(sensorDeviceType);
                    }
                    else
                    {
                        existingType.Name = sensorDeviceType.Name;
                        existingType.Manufacturer = sensorDeviceType.Manufacturer;
                        existingType.Sensors = sensorDeviceType.Sensors;
                    }
                    _sensorDeviceTypesChanged = true;
                    break;
                case Constant.TopicSensorDevice:
                    var sensorDevice = JsonSerializer.Deserialize<SensorDevice>(payload, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    var existingDevice = _sensorDevices.FirstOrDefault(s => s.Id.Equals(sensorDevice.Id));
                    if (existingDevice == null)
                    {
                        _sensorDevices.Add(sensorDevice);
                    }
                    else
                    {
                        existingDevice.Location = sensorDevice.Location;
                        existingDevice.SensorDeviceTypeId = sensorDevice.SensorDeviceTypeId;
                    }
                    _sensorDevicesChanged = true;
                    break;
            }
        }
    }
}
