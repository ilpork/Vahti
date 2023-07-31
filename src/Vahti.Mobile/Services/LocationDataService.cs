﻿using Vahti.Mobile.Forms.Models;
using Vahti.Shared.Data;
using Vahti.Shared.DataProvider;
using Vahti.Shared.Enum;
using Vahti.Shared.TypeData;

namespace Vahti.Mobile.Forms.Services
{
    /// <summary>
    /// Data service used to get and handle current location data
    /// </summary>
    public class LocationDataService : IDataService<Models.Location>
    {
        private readonly IDataProvider _dataProvider;
        private List<Models.Location> _locations;
        private bool _locationPropertyChanged = false;

        public LocationDataService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;            
        }

        public async Task<IReadOnlyList<Models.Location>> GetAllDataAsync(bool forceRefresh)
        {
            await LoadAllItemsIfNeeded(forceRefresh);

            return _locations;         
        }

        public async Task<Models.Location> GetDataAsync(string id, bool forceRefresh)
        {
            await LoadAllItemsIfNeeded(forceRefresh);

            return _locations.FirstOrDefault(l => l.Name.Equals(id));
        }

        public Task UpdateAsync(Models.Location location)
        {
            Preferences.Set(GetLocationOrderKeyName(location.Name), location.Order);

            foreach (var measurement in location)
            {
                Preferences.Set(GetOverviewVisibilityKeyName(location.Name, measurement.SensorId), measurement.IsVisibleInSummaryView);
                Preferences.Set(GetWidgetVisibilityKeyName(location.Name, measurement.SensorId), measurement.IsVisibleInWidget);                
            }

            _locationPropertyChanged = true;
            return Task.CompletedTask;
        }

        private async Task LoadAllItemsIfNeeded(bool forceRefresh)
        {            
            var updatedNeeded = forceRefresh || _locationPropertyChanged || _locations == null || _locations.Count == 0;
            
            if (!updatedNeeded)
            {
                foreach (var location in _locations)
                {
                    if (location != null && (location.Timestamp + TimeSpan.FromMinutes(location.UpdateInterval)) < DateTime.Now)
                        updatedNeeded = true;
                }
            }
            
            if (updatedNeeded)
            {
                await LoadAll();
            }
            _locationPropertyChanged = false;
        }

        private string GetOverviewVisibilityKeyName(string locationName, string measurementName)
        {
            return $"OverviewVisibility_{locationName}£${measurementName}";
        }

        private string GetWidgetVisibilityKeyName(string locationName, string measurementName)
        {
            return $"WidgetVisibility_{locationName}£${measurementName}";
        }

        private string GetLocationOrderKeyName(string locationName)
        {
            return $"LocationOrder_{locationName}";
        }

        private async Task<IEnumerable<Models.Location>> LoadAll()
        {
            if (_locations == null)
            {
                _locations = new List<Models.Location>();
            }
            else
            {
                _locations.Clear();
            }            

            var sensorDeviceTypes = (await _dataProvider.LoadAllItemsAsync<SensorDeviceType>()).ToDictionary(p => p.Id);
            var sensorDevices = (await _dataProvider.LoadAllItemsAsync<SensorDevice>()).ToDictionary(p => p.Id);
            var unsortedList = new List<Models.Location>();

            foreach (var locationData in await _dataProvider.LoadAllItemsAsync<LocationData>())
            {
                var measurements = new List<Measurement>();

                foreach (var m in locationData.Measurements)
                {                    
                    string unit;
                    string sensorName;
                    var sensor = sensorDeviceTypes[sensorDevices[m.SensorDeviceId].SensorDeviceTypeId].Sensors.FirstOrDefault(t => t.Id.Equals(m.SensorId, StringComparison.OrdinalIgnoreCase));
                    var succeeded = Enum.TryParse<SensorClass>(sensor?.Class, true, out var category);
                                        
                    if (succeeded)
                    {
                        unit = sensorDeviceTypes[sensorDevices[m.SensorDeviceId].SensorDeviceTypeId].Sensors.FirstOrDefault(t => t.Id.Equals(m.SensorId, StringComparison.OrdinalIgnoreCase)).Unit;
                        sensorName = sensor.Name;
                    }                    
                    else
                    {
                        // A custom measurement
                        var custom = sensorDevices[m.SensorDeviceId].CalculatedMeasurements?.FirstOrDefault(c => c.Id.Equals(m.SensorId, StringComparison.OrdinalIgnoreCase));
                        Enum.TryParse<SensorClass>(custom.Class, true, out category);
                        unit = custom.Unit;
                        sensorName = custom.Name;
                    }                    
                    
                    measurements.Add(new Measurement()
                    {
                        SensorId = m.SensorId,
                        SensorDeviceId = m.SensorDeviceId,
                        SensorClass = category,
                        Unit = unit,
                        Value = m.Value,
                        SensorName = sensorName,
                        IsVisibleInSummaryView = Preferences.Get(GetOverviewVisibilityKeyName(locationData.Name, m.SensorId), true),
                        IsVisibleInWidget = Preferences.Get(GetWidgetVisibilityKeyName(locationData.Name, m.SensorId), false)
                    });                  

                }

                var location = new Models.Location(locationData.Name, locationData.Timestamp, locationData.UpdateInterval, 
                    measurements, Preferences.Get(GetLocationOrderKeyName(locationData.Name), 0));                
                unsortedList.Add(location);
            }

            foreach (var location in unsortedList.OrderBy(i => i.Order).ThenBy(i => i.Name))
            {                
                _locations.Add(location);
            }

            foreach (var location in _locations)
            {                
                if (_locations.IndexOf(location) != location.Order)
                {
                    location.Order = _locations.IndexOf(location);
                    await UpdateAsync(location);
                }
            }

            return _locations;
        }
    }
}