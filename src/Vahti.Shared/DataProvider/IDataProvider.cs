using System.Collections.Generic;
using System.Threading.Tasks;
using Vahti.Shared.Data;

namespace Vahti.Shared.DataProvider
{
    /// <summary>
    /// Defines functionality of data providers
    /// </summary>
    public interface IDataProvider
    {            

        Task<IEnumerable<T>> LoadAllItemsAsync<T>();

        Task StoreNewItemAsync<T>(T newItem) where T : BaseData;

        Task DeleteItemAsync<T>(T item) where T : BaseData;

        void SetConfiguration(string databaseUrl, string databaseSecret);
    }
}
