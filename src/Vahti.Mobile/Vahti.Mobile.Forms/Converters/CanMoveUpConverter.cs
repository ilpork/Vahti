namespace Vahti.Mobile.Forms.Converters
{
    /// <summary>
    /// Converts <see cref="Models.Location"/> to boolean indicating if item can be moved up in the list
    /// </summary>
    public class CanMoveUpConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is int))
            {
                return false;
            }

            return (int)value > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
