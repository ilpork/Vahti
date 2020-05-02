using System.Collections.Generic;
using System.Threading.Tasks;
using Vahti.Shared.Data;
using Vahti.Shared.TypeData;

namespace Vahti.Collector.DeviceDataReader
{
    /// <summary>
    /// Defines functionality to reading data from device
    /// </summary>
    public interface IDeviceDataReader
    {
        Task<IList<MeasurementData>> ReadDeviceDataAsync(SensorDevice sensorDevice);
    }
}
