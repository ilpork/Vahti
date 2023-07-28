using Vahti.Mobile.Forms.Models;
using Vahti.Shared.Enum;

namespace Vahti.Mobile.Forms.Services
{
    public interface IGraphService
    {
        ChartModel GetChart(MeasurementHistory historyData, SensorClass sensorClass, string sensorName, bool showMinMax);
    }
}
