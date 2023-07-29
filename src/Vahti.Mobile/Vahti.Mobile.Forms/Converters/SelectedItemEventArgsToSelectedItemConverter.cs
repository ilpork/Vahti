using System.Globalization;

namespace Vahti.Mobile.Forms.Converters
{
    /// <summary>
    /// Converts <see cref="SelectedItemChangedEventArgs"/> to object representing the selected item
    /// </summary>
    public class SelectedItemEventArgsToSelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = value as SelectedItemChangedEventArgs;
            return eventArgs.SelectedItem;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}