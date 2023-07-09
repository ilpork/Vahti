using System.Globalization;
using Vahti.Mobile.Forms.Resources;

namespace Vahti.Mobile.Forms.Converters
{
    /// <summary>
    /// Converts seconds to string indicating time since "last charged"
    /// </summary>
    public class SecondsToLastChargedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var secondsPassed = int.Parse(value.ToString(), CultureInfo.InvariantCulture);        
            if (secondsPassed == -1)
            {
                return AppResources.Converter_SecondsToLastCharged_NoInfo;
            }
            else if (secondsPassed == 0)
            {
                return AppResources.Converter_SecondsToLastCharged_Charging;
            }
            else
            {
                return string.Format(AppResources.Converter_SecondsToLastCharged_Mowing, secondsPassed / 60);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
