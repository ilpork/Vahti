using Xamarin.Essentials;
using Xamarin.Forms;

namespace Vahti.Mobile.Forms.Theme
{
    /// <summary>
    /// Provides functionality to change color theme of the application
    /// </summary>
    public class ColorTheme
    {
        public const string ColorThemePreferenceName = "ColorTheme";
        private readonly IThemeChanger _themeChanger;

        public ColorTheme(IThemeChanger themeChanger)
        {
            _themeChanger = themeChanger;
        }
        public ColorThemeEnum ApplyColorTheme()
        {
            var colorTheme = (ColorThemeEnum)Preferences.Get(ColorThemePreferenceName, 0);                       
            
            Application.Current.Resources["ThemeBackground"] = Application.Current.Resources[$"{colorTheme}ThemeBackground"];
            Application.Current.Resources["ThemePrimaryDark"] = Application.Current.Resources[$"{colorTheme}ThemePrimaryDark"];
            Application.Current.Resources["ThemePrimary"] = Application.Current.Resources[$"{colorTheme}ThemePrimary"];
            Application.Current.Resources["ThemeOnPrimary"] = Application.Current.Resources[$"{colorTheme}ThemeOnPrimary"];
            Application.Current.Resources["ThemeSecondary"] = Application.Current.Resources[$"{colorTheme}ThemeSecondary"];
            Application.Current.Resources["ThemeOnSecondary"] = Application.Current.Resources[$"{colorTheme}ThemeOnSecondary"];
            Application.Current.Resources["ThemeSurface"] = Application.Current.Resources[$"{colorTheme}ThemeSurface"];
            Application.Current.Resources["ThemeOnSurface"] = Application.Current.Resources[$"{colorTheme}ThemeOnSurface"];
            Application.Current.Resources["ThemeUnselectedTabText"] = Application.Current.Resources[$"{colorTheme}ThemeUnselectedTabText"];                    
            
            _themeChanger.ApplyTheme(colorTheme);
            
            return colorTheme;
        }
    }
}
