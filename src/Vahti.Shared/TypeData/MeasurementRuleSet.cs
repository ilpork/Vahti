using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using Vahti.Shared.Enum;

namespace Vahti.Shared.TypeData
{
    /// <summary>
    /// Represents set of measurement rules
    /// </summary>
    public class MeasurementRuleSet
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MeasurementRuleSetType Type { get; set; }
        public List<AlertRule> Rules { get; set; }
    }

}

