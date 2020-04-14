using System;
using System.Collections.Generic;

namespace Vahti.Shared.Data
{
    /// <summary>
    /// Represents measurement data related to specific location
    /// </summary>
    public class LocationData : BaseData
    {
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
        public int UpdateInterval { get; set; }

        public List<MeasurementData> Measurements { get; set; }

        public LocationData()
        {
            Measurements = new List<MeasurementData>();            
        }
    }
}
