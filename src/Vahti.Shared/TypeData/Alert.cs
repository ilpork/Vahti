namespace Vahti.Shared.TypeData
{
    /// <summary>
    /// Represents an alert
    /// </summary>
    public class Alert
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public MeasurementRuleSet RuleSet { get; set; }
    }

}

