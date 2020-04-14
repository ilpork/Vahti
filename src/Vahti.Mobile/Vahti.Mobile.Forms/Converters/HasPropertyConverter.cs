using System;
using Xamarin.Forms;

namespace Vahti.Mobile.Forms.Converters
{
    /// <summary>
    /// Converts object to boolean indicating if object has specified property
    /// </summary>
    public class HasPropertyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var type = value.GetType();
            return type.GetProperty(parameter.ToString()) != null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
