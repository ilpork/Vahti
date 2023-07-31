#if ANDROID
using Android.Content;
using Android.Views;
using Microsoft.Maui.Platform;
using Vahti.Mobile.Constants;
#endif

namespace Vahti.Mobile.Theme
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
            Android.App.Application.Context.SetTheme(
                currentTheme == ColorTheme.Light ? Resource.Style.LightTheme : Resource.Style.GrayTheme);            
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

                if (GetCurrentTheme() == ColorTheme.Light)
                {
                    window.SetStatusBarColor(
                        ((Color)Application.Current.Resources[ResourceNames.LightThemePrimaryDark]).ToPlatform());
                    window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                }
                else
                {
                    window.SetStatusBarColor(
                        ((Color)Application.Current.Resources[ResourceNames.GrayThemePrimaryDark]).ToPlatform());
                    window.DecorView.SystemUiVisibility = 0;
                }   
            }            
#endif
        }

        public static ColorTheme GetCurrentTheme()
        {
            return (ColorTheme) Preferences.Get(ColorThemePreferenceName, 0);
        }
    }
}
