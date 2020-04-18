using System.Collections.Generic;
using Vahti.Shared.TypeData;

namespace Vahti.Collector.Configuration
{
    /// <summary>
    /// Represents configuration of <see cref="CollectorService"/>
    /// </summary>
    public partial class CollectorConfiguration
    {
        public bool CollectorEnabled { get; set; }

        public string MqttServerAddress { get; set; }

        public int ScanIntervalSeconds { get; set; }

        public string BluetoothAdapterName { get; set; }

        public List<SensorDevice> SensorDevices { get; set; }

        public List<SensorDeviceType> SensorDeviceTypes { get; set; }
    }
}
