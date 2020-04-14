using Vahti.Mobile.Forms.Localization;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;

namespace Vahti.Mobile.Forms.Services
{
    /// <summary>
    /// Service used to access translated resource strings
    /// </summary>
    public class LanguageService : ILanguageService
    {
        readonly CultureInfo ci = null;
        const string ResourceId = "Vahti.Mobile.Forms.Resources.AppResources";

        static readonly Lazy<ResourceManager> ResMgr = new Lazy<ResourceManager>(
            () => new ResourceManager(ResourceId, typeof(LanguageService).GetTypeInfo().Assembly));

        public LanguageService()
        {
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }
        }
        public string GetString(string id)
        {
            if (id == null)
                return string.Empty;

            var translation = ResMgr.Value.GetString(id, ci);
            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    string.Format("Key '{0}' was not found in resources '{1}' for culture '{2}'.", id, ResourceId, ci.Name),
                    "Text");
#else
                translation = id; // HACK: returns the key, which GETS DISPLAYED TO THE USER
#endif
            }
            return translation;
        }
    }
}
