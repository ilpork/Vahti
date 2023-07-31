namespace Vahti.Mobile.Services
{
    /// <summary>
    /// Defines interface for loading and updating data 
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    public interface IDataService<T>
    {
        /// <summary>
        /// Retrieves available all data 
        /// </summary>
        /// <param name="forceRefresh">When true, forces refreshing the data; If false, then data is not reloaded from database</param>
        /// <returns>List of objects</returns>
        Task<IReadOnlyList<T>> GetAllDataAsync(bool forceRefresh);

        /// <summary>
        /// Retrieves data of specific object
        /// </summary>
        /// <param name="id">ID of the item which data to retrieve</param>
        /// <param name="forceRefresh">When true, forces refreshing the data; If false, then data is not reloaded from database</param>
        /// <returns>Object if found; null otherwise</returns>
        Task<T> GetDataAsync(string id, bool forceRefresh);

        /// <summary>
        /// Updates data of specific object
        /// </summary>
        /// <param name="item">Item to be updated</param>        
        Task UpdateAsync(T item);
    }
}
