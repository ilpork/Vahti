using System.Globalization;
using Vahti.Mobile.Forms.Models;
using Vahti.Shared.Enum;

namespace Vahti.Mobile.Forms.Localization
{
    /// <summary>
    /// Provides functionality to convert values to format suitable for showing in UI
    /// </summary>
    public static class DisplayValueFormatter
    {
        public static string GetMeasurementDisplayValue(Measurement measurement)
        {
            switch (measurement.SensorClass)
            {                
                case SensorClass.Temperature:
                    return $"{string.Format("{0:0.0}", double.Parse(measurement.Value, CultureInfo.InvariantCulture))} {measurement.Unit}";
                case SensorClass.AccelerationX:
                case SensorClass.AccelerationY:
                case SensorClass.AccelerationZ:
                case SensorClass.BatteryVoltage:
                    return $"{string.Format("{0:0.000}", double.Parse(measurement.Value, CultureInfo.InvariantCulture))} {measurement.Unit}";
                case SensorClass.Humidity:
                case SensorClass.MovementCounter:
                case SensorClass.Pressure:
                default:
                    return $"{string.Format("{0:0}", double.Parse(measurement.Value, CultureInfo.InvariantCulture))} {measurement.Unit}";
            }
        }
    }
}
