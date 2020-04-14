using Vahti.Shared.Enum;

namespace Vahti.Shared.TypeData
{
    /// <summary>
    /// Represents alert rule
    /// </summary>
    public partial class AlertRule
    {
        public string Location { get; set; }
        public string SensorDeviceId { get; set; }
        public string SensorId { get; set; }
        public OperatorType Operator { get; set; }
        public string Value { get; set; }
    }
}

