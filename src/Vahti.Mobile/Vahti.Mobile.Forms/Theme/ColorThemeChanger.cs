#if ANDROID
using Android.Content;
using Android.Views;
using Microsoft.Maui.Platform;
using Vahti.Mobile.Forms.Constants;
#endif

namespace Vahti.Mobile.Forms.Theme
{
    /// <summary>
    /// Provides functionality to change color theme of the application
    /// </summary>
    public static class ColorThemeChanger
    {
        public const string ColorThemePreferenceName = "ColorTheme";        

        public static void ApplyTheme()
        {
            var currentTheme = GetCurrentTheme();
            Application.Current.UserAppTheme = currentTheme == ColorTheme.Light ? AppTheme.Light : AppTheme.Dark;
#if ANDROID        
            Context context = Android.App.Application.Context;
            switch (currentTheme)
            {
                default:
                case ColorTheme.Gray:
                    context.SetTheme(Resource.Style.GrayTheme);                    
                    break;
                case ColorTheme.Light:
                    context.SetTheme(Resource.Style.LightTheme);
                    break;
            }
            UpdateStatusBar();
#endif
        }

        public static void UpdateStatusBar()
        {
#if ANDROID
            if (Platform.CurrentActivity != null)
            {
                var window = Platform.CurrentActivity.Window;
                window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

                var color = GetCurrentTheme() == ColorTheme.Light ?
                    (Color)Application.Current.Resources[ResourceNames.LightThemePrimaryDark] :
                    (Color)Application.Current.Resources[ResourceNames.GrayThemePrimaryDark];
                
                window.SetStatusBarColor(color.ToPlatform());
                
            }            
#endif
        }

        public static ColorTheme GetCurrentTheme()
        {
            return (ColorTheme) Preferences.Get(ColorThemePreferenceName, 0);
        }
    }
}
