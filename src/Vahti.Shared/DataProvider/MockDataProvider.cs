using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Vahti.Shared.Data;
using Vahti.Shared.TypeData;

namespace Vahti.Shared.DataProvider
{
    /// <summary>
    /// Provides mock data for data services
    /// </summary>
    public class MockDataProvider : IDataProvider
    {
        public Task DeleteItemAsync<T>(T item) where T : BaseData
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> LoadAllItemsAsync<T>()
        {
            if (typeof(T) == typeof(SensorDeviceType))
            {
                return Task.FromResult(JsonConvert.DeserializeObject<List<T>>(ReadResource("Vahti.Shared.DataProvider.MockData.SensorDeviceTypes.json")).AsEnumerable());
            }
            else if (typeof(T) == typeof(SensorDevice))
            {
                return Task.FromResult(JsonConvert.DeserializeObject<List<T>>(ReadResource("Vahti.Shared.DataProvider.MockData.SensorDevices.json")).AsEnumerable());
            }
            else if (typeof(T) == typeof(LocationData))
            {
                return Task.FromResult(JsonConvert.DeserializeObject<List<T>>(ReadResource("Vahti.Shared.DataProvider.MockData.Locations.json")).AsEnumerable());
            }
            else if (typeof(T) == typeof(MobileDeviceData))
            {
                return Task.FromResult(JsonConvert.DeserializeObject<List<T>>(ReadResource("Vahti.Shared.DataProvider.MockData.MobileDevices.json")).AsEnumerable());
            }
            else if (typeof(T) == typeof(HistoryData))
            {
                return Task.FromResult(JsonConvert.DeserializeObject<List<T>>(ReadResource("Vahti.Shared.DataProvider.MockData.HistoryData.json")).AsEnumerable());
            }

            return Task.FromResult(default(IEnumerable<T>));
        }

        public Task StoreNewItemAsync<T>(T newItem) where T : BaseData
        {
            return Task.CompletedTask;
        }

        public void SetConfiguration(string databaseUrl, string databaseSecret)
        {
        }

        public string ReadResource(string resourcePath)
        {            
            var assembly = Assembly.GetExecutingAssembly();            

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
