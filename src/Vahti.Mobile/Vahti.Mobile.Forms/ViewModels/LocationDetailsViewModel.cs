using Vahti.Mobile.Forms.Resources;
using Vahti.Mobile.Forms.EventArguments;
using Xamarin.Forms;
using System;
using Xamarin.Essentials;
using System.Threading.Tasks;
using Vahti.Mobile.Forms.Services;
using System.Windows.Input;
using MvvmHelpers.Interfaces;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;

namespace Vahti.Mobile.Forms.ViewModels
{
    /// <summary>
    /// View model for page displaying detailed sensor data
    /// </summary>
    public class LocationDetailsViewModel : BaseViewModel
    {           
        private readonly IDataService<Models.Location> _dataService;
        private Models.Location _location;
        private string _locationName;
        private bool _visibilitySettingsUpdated;

        public ICommand InitializeCommand { get; set; }
        public IAsyncCommand RefreshCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand VisibilityToggledCommand { get; set; }

        public Models.Location Location
        {
            get
            {
                return _location;
            }
            set
            {
                SetProperty(ref _location, value);
            }
        }

        public LocationDetailsViewModel(INavigationService navigationService, IDataService<Models.Location> dataService) : base(navigationService)
        {
            _dataService = dataService;
            NavigationService.NavigatedTo += NavigationService_NavigatedTo;

            InitializeCommand = new Command(() => { _visibilitySettingsUpdated = false; });
            RefreshCommand = new AsyncCommand(async () => await RefreshDataAsync());
            UpdateCommand = new Command(() =>
            {
                if (_visibilitySettingsUpdated)
                {                    
                    _dataService.UpdateAsync(Location);
                    _visibilitySettingsUpdated = false;
                }
            });
            VisibilityToggledCommand = new Command(() =>
            {
                _visibilitySettingsUpdated = true;
            });            
        }

        private async void NavigationService_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            if (e.Page == Constants.PageType.Location)
            {
                _locationName = ((Models.Location)e.Parameter).Name;
                Title = $"{_locationName} ({AppResources.Details_Title})";                
                await RefreshDataAsync();
            }
        }

        public async Task RefreshDataAsync()
        {            
            Location = await _dataService.GetDataAsync(_locationName, false);        
        }        
    }    
}
