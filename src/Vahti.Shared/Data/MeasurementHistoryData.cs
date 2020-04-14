using System;
using System.Collections.Generic;
using System.Text;

namespace Vahti.Shared.Data
{
    /// <summary>
    /// Represents measurement history data of specific sensor of specific device
    /// </summary>
    public class MeasurementHistoryData
    {
        public string SensorDeviceId { get; set; }

        public string SensorId { get; set; }

        public List<HistoryValueData> Values { get; set; }
    }
}
