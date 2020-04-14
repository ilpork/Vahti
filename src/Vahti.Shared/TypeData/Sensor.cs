
namespace Vahti.Shared.TypeData
{
    /// <summary>
    /// Represents sensor of a device
    /// </summary>
    public class Sensor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Unit { get; set; }

        public override string ToString()
        {
            return Id ?? base.ToString();
        }
    }

}

