using System;
using System.Globalization;
using Xamarin.Forms;

namespace Vahti.Mobile.Forms.Converters
{
    /// <summary>
    /// Converts string to double
    /// </summary>
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? 0 : double.Parse(value.ToString(), CultureInfo.InvariantCulture);            
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
