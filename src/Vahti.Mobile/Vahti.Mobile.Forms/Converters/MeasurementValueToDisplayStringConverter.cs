﻿using System;
using Vahti.Mobile.Forms.Localization;
using Vahti.Mobile.Forms.Models;
using Xamarin.Forms;

namespace Vahti.Mobile.Forms.Converters
{
    /// <summary>
    /// Converts <see cref="Measurement"/> to string suitable for showing in UI
    /// </summary>
    public class MeasurementToDisplayStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && value is Measurement)
            {
                var measurement = value as Measurement;
                return DisplayValueFormatter.GetMeasurementDisplayValue(measurement.SensorClass, measurement.Value, measurement.Unit);
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
