using System;
using System.Collections.Generic;
using Vahti.Mobile.Forms.Services;
using Xamarin.Essentials;

namespace Vahti.Mobile.Forms.ViewModels
{
    /// <summary>
    /// View model for page displaying application options
    /// </summary>
    public class OptionsGeneralViewModel : BaseViewModel
    {
        private int _colorThemesSelectedIndex;
        private IDatabaseManagementService _dbManagementService;
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
                    Preferences.Set(Theme.ColorTheme.ColorThemePreferenceName, _colorThemesSelectedIndex);
                                  
                    SelectedColorTheme = ColorThemes[_colorThemesSelectedIndex];
                    OnPropertyChanged(nameof(ColorThemesSelectedIndex));
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
        
        public OptionsGeneralViewModel(INavigationService navigationService, IDatabaseManagementService dbManagementService) : base(navigationService)
        {
            _dbManagementService = dbManagementService;
            ColorThemes.Add(Resources.AppResources.ColorTheme_Gray);
            ColorThemes.Add(Resources.AppResources.ColorTheme_Light);

            if (IsDatabaseConfigurationInAppNeeded)
            {
                DatabaseUrl = _dbManagementService.DatabaseUrl;
                DatabaseSecret = _dbManagementService.DatabaseSecret;
            }            

            var colorThemeIndex = Preferences.Get(Theme.ColorTheme.ColorThemePreferenceName, 0);

            // For backwards compatibility (green theme does not exist anymore, light is now '1')
            ColorThemesSelectedIndex = (colorThemeIndex == 2) ? 1 : colorThemeIndex;

            Title = Resources.AppResources.Options_Title;

            this.PropertyChanged += OptionsViewModel_PropertyChanged;
        }

        private void OptionsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ((App)App.Current).Theme.ApplyColorTheme();
        }
    }
}
