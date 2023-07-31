using Vahti.Mobile.Forms.Services;
using Vahti.Mobile.Forms.Theme;

namespace Vahti.Mobile.Forms.ViewModels
{
    /// <summary>
    /// View model for page displaying application options
    /// </summary>
    public class OptionsGeneralViewModel : BaseViewModel
    {        
        private int _colorThemesSelectedIndex;
        private bool _showMinMaxValues;
        private IDatabaseManagementService _dbManagementService;
        private IOptionService _optionService;
        private string _databaseSecret;
        private string _databaseUrl;
        private string _selectedColorTheme = null;              
        
        public List<string> ColorThemes { get; } = new List<string>();

        public string DatabaseSecret
        {
            get { return _databaseSecret; }
            set
            {
                _dbManagementService.DatabaseSecret = value;
                SetProperty(ref _databaseSecret, value, nameof(DatabaseSecret));
            }
        }

        public string DatabaseUrl
        {
            get { return _databaseUrl; }
            set
            {
                _dbManagementService.DatabaseUrl = value;
                SetProperty(ref _databaseUrl, value, nameof(DatabaseUrl));
            }
        }

        public bool IsDatabaseConfigurationInAppNeeded
        {
            get => !_dbManagementService.HasStaticConfiguration;
        }

        public int ColorThemesSelectedIndex
        {
            get
            {
                return _colorThemesSelectedIndex;
            }
            set
            {
                if (_colorThemesSelectedIndex != value)
                {
                    _colorThemesSelectedIndex = value;
                    Preferences.Set(Theme.ColorThemeChanger.ColorThemePreferenceName, _colorThemesSelectedIndex);
                                        
                    SelectedColorTheme = ColorThemes[_colorThemesSelectedIndex];
                    OnPropertyChanged(nameof(ColorThemesSelectedIndex));                    
                }
            }
        }

        public bool ShowMinMaxValues
        {
            get
            {
                return _showMinMaxValues;
            }
            set
            {
                if (_showMinMaxValues != value)
                {
                    _showMinMaxValues = value;
                    _optionService.ShowMinMaxValues = value;
                    SetProperty(ref _showMinMaxValues, value, nameof(ShowMinMaxValues));
                }
            }
        }

        public string SelectedColorTheme
        {
            get
            {
                return _selectedColorTheme;
            }
            set
            {
                if (_selectedColorTheme != value)
                {
                    _selectedColorTheme = value;                    
                    OnPropertyChanged(nameof(SelectedColorTheme));                    
                }
            }
        }
        
        public OptionsGeneralViewModel(INavigationService navigationService, IDatabaseManagementService dbManagementService,
            IOptionService optionService) : base(navigationService)
        {
            _dbManagementService = dbManagementService;
            _optionService = optionService;

            ColorThemes.Add(Resources.AppResources.ColorTheme_Gray);
            ColorThemes.Add(Resources.AppResources.ColorTheme_Light);

            if (IsDatabaseConfigurationInAppNeeded)
            {
                DatabaseUrl = _dbManagementService.DatabaseUrl;
                DatabaseSecret = _dbManagementService.DatabaseSecret;
            }            

            var colorThemeIndex = Preferences.Get(Theme.ColorThemeChanger.ColorThemePreferenceName, 0);

            // For backwards compatibility (green theme does not exist anymore, light is now '1')
            ColorThemesSelectedIndex = (colorThemeIndex == 2) ? 1 : colorThemeIndex;
            ShowMinMaxValues = optionService.ShowMinMaxValues;
            Title = Resources.AppResources.Options_Title;

            this.PropertyChanged += OptionsViewModel_PropertyChanged;
        }

        private void OptionsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ColorThemesSelectedIndex))
            {
                ColorThemeChanger.ApplyTheme();
            }            
        }
    }
}
