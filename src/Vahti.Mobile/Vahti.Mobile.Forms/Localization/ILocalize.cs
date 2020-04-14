using System.Globalization;

namespace Vahti.Mobile.Forms.Localization
{
    /// <summary>
    /// Interface defining localization functionality that platforms (Android and iOS) need to provide
    /// </summary>
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }
}
