using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vahti.Shared.Configuration;
using Vahti.Shared.Data;
using Vahti.Shared.Exception;
using Vahti.Shared.TypeData;

namespace Vahti.Shared.DataProvider
{

    /// <summary>
    /// Provides access to data in Firebase data store
    /// </summary>
    public class FirebaseDataProvider : IDataProvider
    {
        private FirebaseConfiguration _firebaseConfiguration;
                
        public FirebaseDataProvider(IOptions<FirebaseConfiguration> firebaseConfiguration)
        {
            _firebaseConfiguration = firebaseConfiguration.Value;
        }

        public async Task DeleteItemAsync<T>(T item) where T : BaseData
        {
            var firebaseClient = GetFirebaseClient();
            var type = typeof(T).ToString().Split('.').Last();
            var itemToDelete = (await firebaseClient.Child(type).OnceAsync<T>()).Where(a => a.Object.Id == item.Id).FirstOrDefault();

            if (itemToDelete != null)
            {
                await firebaseClient.Child(type).Child(itemToDelete.Key).DeleteAsync();
            }
        }

        public async Task<IEnumerable<T>> LoadAllItemsAsync<T>()
        {
            var firebaseClient = GetFirebaseClient();
            IEnumerable<T> items = new List<T>();
            var type = typeof(T).ToString().Split('.').Last();

            try
            {
                var @switch = new Dictionary<Type, Func<Task>> {
                {
                    typeof(LocationData), async () => {
                        items = (await firebaseClient.Child(type).OnceAsync<LocationData>()).Select(item =>
                        new LocationData()
                            {
                                Id = item.Object.Id,
                                Name = item.Object.Name,
                                Timestamp = item.Object.Timestamp,
                                UpdateInterval = item.Object.UpdateInterval,
                                Measurements = item.Object.Measurements
                            }).Cast<T>();
                    }
                },
                { typeof(SensorDeviceType), async () => {
                        items = (await firebaseClient.Child(type).OnceAsync<SensorDeviceType>()).Select(item =>
                        new SensorDeviceType()
                            {
                                Id = item.Object.Id,
                                Name = item.Object.Name,
                                Manufacturer = item.Object.Manufacturer,
                                Sensors = item.Object.Sensors
                            }).Cast<T>(); } },
                { typeof(SensorDevice), async () => {
                        items = (await firebaseClient.Child(type).OnceAsync<SensorDevice>()).Select(item =>
                        new SensorDevice()
                            {
                                Id = item.Object.Id,
                                Location = item.Object.Location,
                                SensorDeviceTypeId = item.Object.SensorDeviceTypeId,
                                CalculatedMeasurements = item.Object.CalculatedMeasurements
                            }).Cast<T>(); } },
                { typeof(MobileDeviceData), async () => {
                        items = (await firebaseClient.Child(type).OnceAsync<MobileDeviceData>()).Select(item =>
                        new MobileDeviceData()
                            {
                                Id = item.Object.Id,
                                Name = item.Object.Name,
                            }).Cast<T>(); } },
                { typeof(HistoryData), async () => {
                        items = (await firebaseClient.Child(type).OnceAsync<HistoryData>()).Select(item =>
                        new HistoryData()
                            {
                                Id = item.Object.Id,
                                DataList = item.Object.DataList
                            }).Cast<T>(); } },
                };
    
                await @switch[typeof(T)]();
            }            
            catch (System.Exception ex)
            {
                throw new DatabaseException(_firebaseConfiguration.Url, ex);
            }

            return items;
        }

        public async Task StoreNewItemAsync<T>(T newItem) where T : BaseData
        {
            var firebaseClient = GetFirebaseClient();

            var type = typeof(T).ToString().Split('.').Last();
            var itemToUpdate = (await firebaseClient.Child(type).OnceAsync<T>()).Where(a => a.Object.Id == newItem.Id).FirstOrDefault();

            if (itemToUpdate == null)
            {
                await firebaseClient.Child(type).PostAsync<T>(newItem);
            }
            else
            {
                await firebaseClient.Child(type).Child(itemToUpdate.Key).PutAsync<T>(newItem);
            }            
        }

        public void SetConfiguration(string databaseUrl, string databaseSecret)
        {
            _firebaseConfiguration = new FirebaseConfiguration()
            {
                Enabled = _firebaseConfiguration.Enabled,
                DatabaseSecret = databaseSecret,
                Url = databaseUrl
            };
        }

        private FirebaseClient GetFirebaseClient()
        {
            if (string.IsNullOrEmpty(_firebaseConfiguration.DatabaseSecret))
            {
                return new FirebaseClient(_firebaseConfiguration.Url);
            }
            else
            {
                return new FirebaseClient(_firebaseConfiguration.Url,
                    new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(_firebaseConfiguration.DatabaseSecret) });
            }
        }
    }
}
