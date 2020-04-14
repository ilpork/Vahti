using System.Collections.Generic;
using System.Threading.Tasks;
using Vahti.Shared.Data;

namespace Vahti.DataBroker.DataProvider
{
    /// <summary>
    /// Defines functionality to handle and publish measurement history data
    /// </summary>
    public interface IHistoryDataProvider
    {
        public Task<IReadOnlyList<MeasurementData>> GetHistory(string deviceId, string sensorId, int daysBack);
        public Task DeleteOldHistory(int daysBack);
        public Task AddItems(IList<MeasurementData> data);
    }
}
