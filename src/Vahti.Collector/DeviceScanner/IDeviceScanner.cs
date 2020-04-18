using System.Collections.Generic;
using System.Threading.Tasks;
using Vahti.Shared.Data;
using Vahti.Shared.TypeData;

namespace Vahti.Collector.DeviceScanner
{
    /// <summary>
    /// Defines functionality to scanning devices for measurement data
    /// </summary>
    public interface IDeviceScanner
    {        
        Task<IList<MeasurementData>> GetDeviceDataAsync(IList<SensorDevice> sensorDevices);
    }
}
