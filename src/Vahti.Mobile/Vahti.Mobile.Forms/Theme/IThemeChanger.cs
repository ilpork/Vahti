namespace Vahti.Mobile.Forms.Theme
{
    /// <summary>
    /// Defines functionality that platform must implement to support themes
    /// </summary>
    public interface IThemeChanger
    {
        void ApplyTheme(ColorThemeEnum theme);
    }
}
