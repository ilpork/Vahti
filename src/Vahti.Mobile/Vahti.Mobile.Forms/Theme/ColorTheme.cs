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
                        
            switch (colorTheme)
            {
                case ColorThemeEnum.Gray:
                    Application.Current.Resources["ThemeBackgroundColor"] = Application.Current.Resources["GrayThemeDark"];
                    Application.Current.Resources["ThemeBackgroundLightColor"] = Application.Current.Resources["GrayThemeLight"];
                    Application.Current.Resources["ThemeLocationHeaderColor"] = Application.Current.Resources["GrayThemeLocationHeaderColor"];
                    Application.Current.Resources["ThemeListItemBackgroundColor"] = Application.Current.Resources["GrayThemeListItemBackground"];
                    Application.Current.Resources["ThemeTabHeaderBackgroundColor"] = Application.Current.Resources["GrayThemeTabHeaderBackgroundColor"];
                    Application.Current.Resources["ThemeUnselectedTabTextColor"] = Application.Current.Resources["GrayThemeUnselectedTabTextColor"];
                    Application.Current.Resources["ThemeTextColor"] = Application.Current.Resources["GrayThemeTextColor"];
                    Application.Current.Resources["ThemeShellBackgroundColor"] = Application.Current.Resources["GrayThemeShellBackgroundColor"];
                    Application.Current.Resources["ThemeSwitchThumbOnColor"] = Application.Current.Resources["GrayThemeSwitchThumbOnColor"];
                    Application.Current.Resources["ThemeSwitchThumbColor"] = Application.Current.Resources["GrayThemeSwitchThumbColor"];
                    break;
                case ColorThemeEnum.Green:
                    Application.Current.Resources["ThemeBackgroundColor"] = Application.Current.Resources["GreenThemeDark"];
                    Application.Current.Resources["ThemeBackgroundLightColor"] = Application.Current.Resources["GreenThemeLight"];
                    Application.Current.Resources["ThemeLocationHeaderColor"] = Application.Current.Resources["GreenThemeLocationHeaderColor"];
                    Application.Current.Resources["ThemeListItemBackgroundColor"] = Application.Current.Resources["GreenThemeListItemBackground"];
                    Application.Current.Resources["ThemeTabHeaderBackgroundColor"] = Application.Current.Resources["GreenThemeTabHeaderBackgroundColor"];
                    Application.Current.Resources["ThemeUnselectedTabTextColor"] = Application.Current.Resources["GreenThemeUnselectedTabTextColor"];
                    Application.Current.Resources["ThemeTextColor"] = Application.Current.Resources["GreenThemeTextColor"];
                    Application.Current.Resources["ThemeShellBackgroundColor"] = Application.Current.Resources["GreenThemeShellBackgroundColor"];
                    Application.Current.Resources["ThemeSwitchThumbOnColor"] = Application.Current.Resources["GreenThemeSwitchThumbOnColor"];
                    Application.Current.Resources["ThemeSwitchThumbColor"] = Application.Current.Resources["GreenThemeSwitchThumbColor"];
                    break;
                case ColorThemeEnum.Light:
                    Application.Current.Resources["ThemeBackgroundColor"] = Application.Current.Resources["LightThemeDark"];
                    Application.Current.Resources["ThemeBackgroundLightColor"] = Application.Current.Resources["LightThemeLight"];
                    Application.Current.Resources["ThemeLocationHeaderColor"] = Application.Current.Resources["LightThemeLocationHeaderColor"];
                    Application.Current.Resources["ThemeListItemBackgroundColor"] = Application.Current.Resources["LightThemeListItemBackground"];
                    Application.Current.Resources["ThemeTabHeaderBackgroundColor"] = Application.Current.Resources["LightThemeTabHeaderBackgroundColor"];
                    Application.Current.Resources["ThemeUnselectedTabTextColor"] = Application.Current.Resources["LightThemeUnselectedTabTextColor"];
                    Application.Current.Resources["ThemeTextColor"] = Application.Current.Resources["LightThemeTextColor"];
                    Application.Current.Resources["ThemeShellBackgroundColor"] = Application.Current.Resources["LightThemeShellBackgroundColor"];
                    Application.Current.Resources["ThemeSwitchThumbOnColor"] = Application.Current.Resources["LightThemeSwitchThumbOnColor"];
                    Application.Current.Resources["ThemeSwitchThumbColor"] = Application.Current.Resources["LightThemeSwitchThumbColor"];
                    break;
            }
            _themeChanger.ApplyTheme(colorTheme);
            
            return colorTheme;
        }
    }
}
