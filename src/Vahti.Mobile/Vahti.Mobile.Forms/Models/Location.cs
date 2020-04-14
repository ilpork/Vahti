using System;
using System.Collections.Generic;

namespace Vahti.Mobile.Forms.Models
{
    /// <summary>
    /// Represents a location containing measurements shown in UI
    /// </summary>
    public class Location : List<Measurement>
    {
        public string Name { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int UpdateInterval { get; private set; }

        public Location(string name, DateTime timestamp, int updateInterval, List<Measurement> measurements) : base(measurements)
        {
            Name = name;
            Timestamp = timestamp;
            UpdateInterval = updateInterval;
        }
    }
}
