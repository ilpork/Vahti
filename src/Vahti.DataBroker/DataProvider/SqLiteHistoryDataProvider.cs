using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vahti.Shared.Data;

namespace Vahti.DataBroker.DataProvider
{
    /// <summary>
    /// Provides functionality to access and handle measurement history data (local SQLite database)
    /// </summary>
    public class SqLiteHistoryDataProvider : IHistoryDataProvider, IDisposable
    {        
        private bool _initialized = false;
        private bool _isDisposed = false;
        private SQLiteAsyncConnection _connection = null;
      
        public async Task AddItems(IList<MeasurementData> data)
        {
            await Initialize();
            await _connection.InsertAllAsync(data, true);
        }

        protected async virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_connection != null)
                    {
                        await _connection.CloseAsync();
                    }
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public async Task<IReadOnlyList<MeasurementData>> GetHistory(string deviceId, string sensorId, int daysBack)
        {
            await Initialize();
            return await _connection.QueryAsync<MeasurementData>("select * from MeasurementData where Timestamp > ? and SensorDeviceId = ? and SensorId = ?", DateTime.Now.Ticks - TimeSpan.TicksPerDay * daysBack, deviceId, sensorId);
        }

        public async Task DeleteOldHistory(int preserveDaysBack)
        {
            await Initialize();
            await _connection.ExecuteAsync("delete from MeasurementData where Timestamp < ?", DateTime.Now.Ticks - TimeSpan.TicksPerDay * preserveDaysBack * 2);
        }

        public async Task Flush()
        {
            await Initialize();
            await _connection.DeleteAllAsync<MeasurementData>();
        }

        private async Task Initialize()
        {
            if (!_initialized)
            {
                _connection = new SQLiteAsyncConnection(SqLiteDataProvider.DatabaseName);
                await _connection.CreateTableAsync<MeasurementData>();
                _initialized = true;
            }            
        }
    }
}
