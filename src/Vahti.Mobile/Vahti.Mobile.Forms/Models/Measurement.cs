using Vahti.Shared.Enum;

namespace Vahti.Mobile.Forms.Models
{
    /// <summary>
    /// Represents a measurement shown in user interface
    /// </summary>
    public class Measurement
    {
        public string SensorId { get; set; }
        public string SensorDeviceId { get; set; }
        public SensorClass SensorClass { get; set; }
        public string SensorName { get; set; }
        public string Unit { get; set; }
        public string Value { get; set; }
        public bool IsVisible { get; set; }
    }
}
