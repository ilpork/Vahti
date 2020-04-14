using Vahti.Shared.Enum;

namespace Vahti.Shared.TypeData
{
    /// <summary>
    /// Represents a custom measurement rule
    /// </summary>
    public partial class CustomMeasurementRule
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Unit { get; set; }
        public ValueType Type { get; set; }
        public string SensorId { get; set; }
        public OperatorType Operator { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return Id ?? base.ToString();
        }
    }

}

