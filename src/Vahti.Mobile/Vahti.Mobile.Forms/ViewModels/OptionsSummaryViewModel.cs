using Vahti.Mobile.Forms.Resources;
using Xamarin.Forms;
using System.Threading.Tasks;
using Vahti.Mobile.Forms.Services;
using System.Collections.Generic;

namespace Vahti.Mobile.Forms.ViewModels
{
    /// <summary>
    /// View model for page used to choose what data to show in summary UI
    /// </summary>
    public class OptionsSummaryViewModel : BaseViewModel
    {
        private readonly IDataService<Models.Location> _dataService;                
        private bool _visibilitySettingsUpdated;        
        private IReadOnlyList<Models.Location> _locationList;

        public Command InitializeCommand { get; set; }
        public Command RefreshCommand { get; set; }
        public Command UpdateCommand { get; set; }
        public Command VisibilityToggledCommand { get; set; }       

        public IReadOnlyList<Models.Location> Locations
        {
            get
            {
                return _locationList;
            }
            set
            {
                SetProperty(ref _locationList, value);
            }
        }

        public OptionsSummaryViewModel(INavigationService navigationService, IDataService<Models.Location> dataService) : base(navigationService)
        {
            _dataService = dataService;            

            InitializeCommand = new Command(async () => 
            {
                await RefreshDataAsync();
                _visibilitySettingsUpdated = false; 
            });

            RefreshCommand = new Command(async () => await RefreshDataAsync());
            UpdateCommand = new Command(() =>
            {
                if (_visibilitySettingsUpdated)
                {
                    foreach (var location in Locations)
                    {
                        _dataService.UpdateAsync(location);
                        _visibilitySettingsUpdated = false;                 
                    }                 
                }               
            });
            VisibilityToggledCommand = new Command(() =>
            {
                _visibilitySettingsUpdated = true;
            });           

            Title = AppResources.Options_Title;
        }       

        public async Task RefreshDataAsync()
        {
            try
            {
                IsBusy = true;
                Locations = await _dataService.GetAllDataAsync(false);
            }
            finally
            {
                IsBusy = false;
            }
            
        }
    }
}
