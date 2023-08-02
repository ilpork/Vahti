using Vahti.Mobile.Models;
using Vahti.Shared.Enum;

namespace Vahti.Mobile.Services
{
    public interface IGraphService
    {
        ChartModel GetChart(MeasurementHistory historyData, SensorClass sensorClass, string sensorName, bool showMinMax);
    }
}
