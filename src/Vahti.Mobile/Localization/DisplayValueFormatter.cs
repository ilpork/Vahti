using System.Globalization;
using Vahti.Shared.Enum;

namespace Vahti.Mobile.Forms.Localization
{
    /// <summary>
    /// Provides functionality to convert values to format suitable for showing in UI
    /// </summary>
    public static class DisplayValueFormatter
    {
        public static string GetMeasurementDisplayValue(SensorClass sensorClass, string value, string unit)
        {
            var valueWithoutUnit = GetMeasurementDisplayValue(sensorClass, value);

            if (!string.IsNullOrEmpty(valueWithoutUnit))
            {
                valueWithoutUnit += $" {unit}";
            }

            return valueWithoutUnit;
        }

        public static string GetMeasurementDisplayValue(SensorClass sensorClass, string value)
        {
            switch (sensorClass)
            {
                case SensorClass.None:
                    return string.Empty;
                case SensorClass.Temperature:
                    return $"{string.Format("{0:0.0}", double.Parse(value, CultureInfo.InvariantCulture))}";
                case SensorClass.AccelerationX:
                case SensorClass.AccelerationY:
                case SensorClass.AccelerationZ:
                case SensorClass.BatteryVoltage:
                    return $"{string.Format("{0:0.000}", double.Parse(value, CultureInfo.InvariantCulture))}";
                case SensorClass.Humidity:
                case SensorClass.MovementCounter:
                case SensorClass.Pressure:
                default:
                    return $"{string.Format("{0:0}", double.Parse(value, CultureInfo.InvariantCulture))}";
            }
        }
    }
}
