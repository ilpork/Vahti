using System.Globalization;
using System.Threading;
using Vahti.Mobile.Forms.Localization;
using Xamarin.Forms;

[assembly: Dependency(typeof(Vahti.Mobile.Droid.Localize))]
namespace Vahti.Mobile.Droid
{
    /// <summary>
    /// Provides Android specific localization support
    /// </summary>
    public class Localize : ILocalize
    {
        private CultureInfo _ci;

        public void SetLocale(CultureInfo ci)
        {
            _ci = Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = ci;             
        }

        public CultureInfo GetCurrentCultureInfo()
        {            
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = AndroidToDotnetLanguage(androidLocale.ToString().Replace("_", "-"));

            CultureInfo ci;
            try
            {
                ci = new CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException)
            {
                // Use fallback culture
                try
                {
                    var fallback = ToDotnetFallbackLanguage(new PlatformCulture(netLanguage));
                    ci = new CultureInfo(fallback);
                }
                catch (CultureNotFoundException)
                {                    
                    ci = new CultureInfo("en");
                }
            }
            return ci;
        }
        private string AndroidToDotnetLanguage(string androidLanguage)
        {
            var netLanguage = androidLanguage;
            
            //certain languages need to be converted to CultureInfo equivalent
            switch (androidLanguage)
            {
                case "ms-BN":   // "Malaysian (Brunei)" not supported .NET culture
                case "ms-MY":   // "Malaysian (Malaysia)" not supported .NET culture
                case "ms-SG":   // "Malaysian (Singapore)" not supported .NET culture
                    netLanguage = "ms"; // closest supported
                    break;
                case "in-ID":  // "Indonesian (Indonesia)" has different code in  .NET
                    netLanguage = "id-ID"; // correct code for .NET
                    break;
                case "gsw-CH":  // "Schwiizertüütsch (Swiss German)" not supported .NET culture
                    netLanguage = "de-CH"; // closest supported
                    break;            
            }
            return netLanguage;
        }
        private string ToDotnetFallbackLanguage(PlatformCulture platCulture)
        {
            var netLanguage = platCulture.LanguageCode; // use the first part of the identifier (two chars, usually);
            switch (platCulture.LanguageCode)
            {
                case "gsw":
                    netLanguage = "de-CH"; // equivalent to German (Switzerland) for this app
                    break;                    
            }
            return netLanguage;
        }
    }
}