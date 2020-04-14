using System.Collections.Generic;

namespace Vahti.Shared.Data
{
    /// <summary>
    /// Represents history data
    /// </summary>
    public class HistoryData : BaseData
    {
        public List<MeasurementHistoryData> DataList { get; set; }
    }
}
