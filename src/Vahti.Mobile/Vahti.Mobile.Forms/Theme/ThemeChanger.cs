#if ANDROID
using Android.Util;
using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Runtime;
using Android.OS;
#endif

namespace Vahti.Mobile.Forms.Theme
{
    /// <summary>
    /// Defines functionality that platform must implement to support themes
    /// </summary>
    public class ThemeChanger : IThemeChanger
    {
        public void ApplyTheme(ColorThemeEnum theme)
        {
#if ANDROID        
            Context context = Android.App.Application.Context;
            switch (theme)
            {
                default:
                case ColorThemeEnum.Gray:                                        
                    context.SetTheme(Resource.Style.GrayTheme);
                    break;                
                case ColorThemeEnum.Light:
                    context.SetTheme(Resource.Style.LightTheme);
                    break;
            }        
#endif
        }
    }

    public interface IThemeChanger
    {
        public void ApplyTheme(ColorThemeEnum theme);
    }
}
