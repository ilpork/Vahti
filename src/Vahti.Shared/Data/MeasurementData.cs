using System;

namespace Vahti.Shared.Data
{
    /// <summary>
    /// Represents measurement data of specific sensor of specific device
    /// </summary>
    public class MeasurementData
    {
        public DateTime Timestamp { get; set; }
        public string SensorDeviceId { get; set; }
        public string SensorId { get; set; }
        public string Value { get; set; }
    }
}
