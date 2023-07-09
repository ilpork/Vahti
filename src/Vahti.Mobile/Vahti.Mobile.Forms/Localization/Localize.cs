using System.Globalization;

namespace Vahti.Mobile.Forms.Localization
{    
    /// <summary>
    /// Provides Android specific localization support
    /// </summary>
    /// TODO: add iOS support
    public class Localize
    {
        private CultureInfo _ci;

        public void SetLocale(CultureInfo ci)
        {
#if ANDROID
            _ci = Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = ci;
#endif
        }

        public CultureInfo GetCurrentCultureInfo()
        {
            CultureInfo ci = null;
#if ANDROID
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = AndroidToDotnetLanguage(androidLocale.ToString().Replace("_", "-"));
      
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
#endif
            return ci;
        }
        private string AndroidToDotnetLanguage(string androidLanguage)
        {
            var netLanguage = androidLanguage;

#if ANDROID
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
#endif
            return netLanguage;
        }
        private string ToDotnetFallbackLanguage(PlatformCulture platCulture)
        {
            var netLanguage = platCulture.LanguageCode; // use the first part of the identifier (two chars, usually);
#if ANDROID            
            switch (platCulture.LanguageCode)
            {
                case "gsw":
                    netLanguage = "de-CH"; // equivalent to German (Switzerland) for this app
                    break;
            }
#endif
            return netLanguage;
        }
    }
}
