using System.Globalization;

namespace Vahti.Mobile.Converters
{
    /// <summary>
    /// Converts <see cref="Models.Location"/> to boolean indicating if item can be moved down in the list
    /// </summary>
    public class CanMoveDownConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2 || !(values[0] is int) || !(values[1] is CollectionView))
            {
                return false;
            }

            var order = (int)values[0];
            var locations = ((CollectionView)values[1]).ItemsSource.Cast<Models.Location>();

            return order < locations.Count() - 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
