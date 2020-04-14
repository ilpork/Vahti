using System.Collections.Generic;
using Vahti.Shared.Data;

namespace Vahti.Shared.TypeData
{
    /// <summary>
    /// Represents a device which has sensors
    /// </summary>
    public partial class SensorDevice : BaseData
    {
        public string SensorDeviceTypeId { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public List<CustomMeasurementRule> CalculatedMeasurements { get; set; }

        public override string ToString()
        {
            return $"{Id} ({SensorDeviceTypeId})" ?? base.ToString();
        }
    }

}

