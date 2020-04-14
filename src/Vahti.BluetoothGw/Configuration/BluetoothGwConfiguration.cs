using System.Collections.Generic;
using Vahti.Shared.TypeData;

namespace Vahti.BluetoothGw.Configuration
{
    /// <summary>
    /// Represents configuration of <see cref="BluetoothGwService"/>
    /// </summary>
    public partial class BluetoothGwConfiguration
    {
        public bool BluetoothGwEnabled { get; set; }

        public string MqttServerAddress { get; set; }

        public int ScanIntervalSeconds { get; set; }

        public string AdapterName { get; set; }

        public List<SensorDevice> SensorDevices { get; set; }

        public List<SensorDeviceType> SensorDeviceTypes { get; set; }
    }
}
