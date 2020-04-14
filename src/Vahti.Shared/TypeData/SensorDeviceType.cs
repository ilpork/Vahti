using System.Collections.Generic;
using Vahti.Shared.Data;

namespace Vahti.Shared.TypeData
{
    /// <summary>
    /// Represents type of device having sensors
    /// </summary>
    public class SensorDeviceType : BaseData
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public List<Sensor> Sensors { get; set; }

        public override string ToString()
        {
            return Name ?? base.ToString();
        }
    }
}
