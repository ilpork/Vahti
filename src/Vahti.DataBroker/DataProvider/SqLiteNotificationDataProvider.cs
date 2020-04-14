using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vahti.Shared.Data;

namespace Vahti.DataBroker.DataProvider
{
    /// <summary>
    /// Provides access to local SQLite database containing notification related data
    /// </summary>
    public class SqLiteNotificationDataProvider : INoticationDataProvider, IDisposable
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

        public async Task<bool> HasNotificationBeenSent(string alertId)
        {
            await Initialize();

            var notification = (await _connection.QueryAsync<NotificationData>("select * from NotificationData where Id = ?", alertId)).FirstOrDefault();

            if (notification == null)
            {
                return false;
            }
            else
            {
                return notification.IsSent;
            }
        }
        public async Task SetNotificationStatus(string alertId, bool isSent)
        {
            await Initialize();
            await _connection.InsertOrReplaceAsync(new NotificationData() { Id = alertId, IsSent = isSent });
        }

        private async Task Initialize()
        {
            if (!_initialized)
            {
                _connection = new SQLiteAsyncConnection(SqLiteDataProvider.DatabaseName);
                await _connection.CreateTableAsync<NotificationData>(CreateFlags.ImplicitPK);
                _initialized = true;
            }
        }
    }
}
