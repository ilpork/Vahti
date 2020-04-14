using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vahti.Mobile.Forms.Models;
using Vahti.Shared.Data;
using Vahti.Shared.DataProvider;
using Vahti.Shared.TypeData;

namespace Vahti.Mobile.Forms.Services
{
    /// <summary>
    /// Provides history data of sensors
    /// </summary>
    public class HistoryDataService : IDataService<MeasurementHistory>
    {
        private readonly IDataProvider _dataProvider;
        private HistoryData _historyData;
        private Dictionary<string, SensorDeviceType> _sensorDeviceTypes;
        private Dictionary<string, SensorDevice> _sensorDevices;
        
        public HistoryDataService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }
        
        public async Task<IReadOnlyList<MeasurementHistory>> GetAllDataAsync(bool forceRefresh)
        {
            if (_historyData == null || forceRefresh)
            {
                _historyData = (await _dataProvider.LoadAllItemsAsync<HistoryData>()).FirstOrDefault();
            }
            if (_sensorDeviceTypes == null || forceRefresh)
            {
                _sensorDeviceTypes = (await _dataProvider.LoadAllItemsAsync<SensorDeviceType>()).ToDictionary(t => t.Id);
                _sensorDevices = (await _dataProvider.LoadAllItemsAsync<SensorDevice>()).ToDictionary(d => d.Id);
            }

            var historyItems = new List<MeasurementHistory>();

            if (_historyData != null)
            {
                foreach (var item in _historyData.DataList)
                {
                    var sensorDeviceType = _sensorDeviceTypes[_sensorDevices[item.SensorDeviceId].SensorDeviceTypeId];
                    var unit = sensorDeviceType.Sensors.FirstOrDefault(s => s.Id.Equals(item.SensorId)).Unit;
                    var uiItem = new MeasurementHistory() { SensorDeviceId = item.SensorDeviceId, SensorId = item.SensorId, Values = item.Values, Unit = unit };

                    historyItems.Add(uiItem);                    
                }
            }

            return historyItems;
        }

        public Task<MeasurementHistory> GetDataAsync(string id, bool forceRefresh)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(MeasurementHistory item)
        {
            throw new NotImplementedException();
        }
    }
}
