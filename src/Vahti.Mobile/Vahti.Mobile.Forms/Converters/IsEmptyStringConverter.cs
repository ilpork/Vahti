using System;
using Xamarin.Forms;

namespace Vahti.Mobile.Forms.Converters
{
    /// <summary>
    /// Converts string to boolean indicating if string is empty
    /// </summary>
    public class IsEmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null && value.ToString().Equals(String.Empty);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
