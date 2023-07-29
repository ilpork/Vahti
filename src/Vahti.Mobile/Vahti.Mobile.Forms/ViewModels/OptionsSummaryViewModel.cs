using Vahti.Mobile.Forms.Resources;
using Vahti.Mobile.Forms.Services;
using MvvmHelpers.Interfaces;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using System.Windows.Input;

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

        public ICommand InitializeCommand { get; set; }
        public IAsyncCommand RefreshCommand { get; set; }
        public IAsyncCommand UpdateCommand { get; set; }
        public ICommand VisibilityToggledCommand { get; set; }       

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

            InitializeCommand = new Command(() => 
            {
                IsBusy = true;
                _visibilitySettingsUpdated = false; 
            });

            RefreshCommand = new AsyncCommand(async () => await RefreshDataAsync());
            UpdateCommand = new AsyncCommand(async () =>
            {
                if (_visibilitySettingsUpdated)
                {
                    foreach (var location in Locations)
                    {
                        await _dataService.UpdateAsync(location);
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
                Locations = await _dataService.GetAllDataAsync(false);
            }
            finally
            {
                IsBusy = false;
            }
            
        }
    }
}
