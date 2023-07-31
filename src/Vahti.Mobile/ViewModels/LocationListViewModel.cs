using System.Collections.ObjectModel;
using System.Diagnostics;
using Vahti.Mobile.Models;
using Vahti.Mobile.Exceptions;
using Vahti.Mobile.Services;
using Vahti.Shared.Exception;
using MvvmHelpers.Interfaces;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using System.Windows.Input;

namespace Vahti.Mobile.ViewModels
{
    /// <summary>
    /// View model for page used to show list the available sensors
    /// </summary>
    public class LocationListViewModel : BaseViewModel
    {
        private bool _hasItems;
        private bool _isOldData;
        private string _lastUpdated;
        private string _noDataMessage;
        private Measurement _selectedMeasurement;
        private readonly IDataService<Models.Location> _locationDataService;
        private readonly IDatabaseManagementService _databaseManagementService;
        private bool _preventUpdate = false;

        public ObservableCollection<Models.Location> Locations { get; set; }
        public IAsyncCommand<bool> RefreshListCommand { get; set; }
        public ICommand SelectItemCommand { get; set; }
        public AsyncCommand<string> TapLocationCommand { get; set; }

        public bool HasItems 
        { 
            get => _hasItems; 
            set => SetProperty(ref _hasItems, value); 
        }

        public bool IsOldData
        {
#if DEBUGMOCK
            get => false;
#else
            get => _isOldData;
#endif
            set => SetProperty(ref _isOldData, value);
        }

        public string LastUpdated
        {
            get => _lastUpdated;                            
            set => SetProperty(ref _lastUpdated, value);
        }
        
        public string NoDataMessage
        {
            get => _noDataMessage;
            set => SetProperty(ref _noDataMessage, value);
        }

        public Measurement SelectedMeasurement
        {
            get =>_selectedMeasurement;            
            set => SetProperty(ref _selectedMeasurement, value);            
        }        

        public LocationListViewModel(IDataService<Models.Location> dataStore, INavigationService navigationService, IDatabaseManagementService databaseManagementService) : base(navigationService)
        {
            _locationDataService = dataStore;
            _databaseManagementService = databaseManagementService;

            Title = Resources.AppResources.App_Title;
            Locations = new ObservableCollection<Models.Location>();            
            RefreshListCommand = new AsyncCommand<bool>(async (forceRefresh) => await RefreshSensorList(forceRefresh));            
            SelectItemCommand = new Command(() =>
            {                
                if (SelectedMeasurement != null)
                {
                    var location = Locations.FirstOrDefault(l => l.Contains(SelectedMeasurement));
                    NavigationService.NavigateTo(Constants.PageType.Location, location);                    
                }                
            });

            TapLocationCommand = new AsyncCommand<string>(async (locationName) =>
            {
                if (locationName != null)
                {
                    var location = Locations.FirstOrDefault(l => l.Name.Equals(locationName, StringComparison.OrdinalIgnoreCase));
                    await NavigationService.NavigateTo(Constants.PageType.Location, location);
                }
            });
        }
     
        private async Task RefreshSensorList(bool forceRefresh)
        {
            if (!_databaseManagementService.HasDatabaseUrlDefined)
            {
                await NavigationService.NavigateTo(Constants.PageType.Options);
                return;
            }

            // Setting IsBusy trigs refresh command (through RefreshView), and this ugly thing prevents the duplicate call
            if (_preventUpdate)
            {
                return;
            }

            IsBusy = _preventUpdate = true;            
            NoDataMessage = null;            

            try
            {                
                Locations.Clear();

                IEnumerable<Models.Location> items = new List<Models.Location>();
                try
                {
                    items = await _locationDataService.GetAllDataAsync(forceRefresh);

                    CheckIfDataIsOld(items);

                    foreach (var location in items)
                    {
                        var filteredLocation = new Models.Location(location.Name, location.Timestamp, location.UpdateInterval, location.ToList(), location.Order);
                        filteredLocation.RemoveAll(m => !m.IsVisibleInSummaryView);

                        // If measurement count is not even, add a dummy measurement to make location group look like rectangle
                        if (filteredLocation.Count % 2 != 0)
                        {
                            var lastMeasurement = filteredLocation.Last();
                            filteredLocation.Add(new Measurement() { IsVisibleInSummaryView = false, SensorName = lastMeasurement.SensorName, 
                                Value = lastMeasurement.Value, Unit = lastMeasurement.Unit, SensorClass = lastMeasurement.SensorClass });
                        }

                        // Small delay is needed on iOS prevent crashing due to a bug in XF                        
                        if (DeviceInfo.Platform == DevicePlatform.iOS)
                        {
                            await Task.Delay(50);
                        }

                        Locations.Add(filteredLocation);
                    }                
                }
                catch (DataNotFoundException)
                {
                    // TODO: if data id is empty, show message to user
                }   
                catch (DatabaseException ex)
                {
                    NoDataMessage = string.Format(Resources.AppResources.Locations_DatabaseProblem, ex.Url);
                }

                HasItems = Locations.Count > 0;                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = _preventUpdate = false;
            }
        }      
        
        private void CheckIfDataIsOld(IEnumerable<Models.Location> locations)
        {
            var latestUpdateDateTime = DateTime.MinValue;
            var locationsNotUpdated = new List<string>();

            foreach (var location in locations)
            {
                // Use tolerance of four times of data update interval to determine if data is old or not
                if (location != null && (location.Timestamp + TimeSpan.FromMinutes(location.UpdateInterval * 4)) < DateTime.Now)
                {
                    locationsNotUpdated.Add(location.Name);
                    if (location.Timestamp > latestUpdateDateTime)
                    {
                        latestUpdateDateTime = location.Timestamp;
                    }
                }
            }

            if (locationsNotUpdated.Count > 0)
            {
                var locationListText = string.Join(", ", locationsNotUpdated);

                LastUpdated = string.Format(Resources.AppResources.Locations_LastUpdatedText, locationListText,
                    $"{latestUpdateDateTime.ToShortDateString()} {latestUpdateDateTime.ToLongTimeString()}");
                IsOldData = true;
            }
            else
            {
                IsOldData = false;
            }
        }
    }
}