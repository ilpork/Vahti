using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Vahti.Mobile.Forms.Models;
using System.Linq;
using Xamarin.Essentials;
using Vahti.Mobile.Forms.Exceptions;
using System.Collections.Generic;
using Vahti.Mobile.Forms.Services;
using Vahti.Shared.Data;
using Vahti.Shared.Exception;

namespace Vahti.Mobile.Forms.ViewModels
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

        public ObservableCollection<Models.Location> Locations { get; set; }
        public Command<bool> RefreshListCommand { get; set; }
        public Command SelectItemCommand { get; set; }
        public Command<string> TapLocationCommand { get; set; }

        public bool HasItems 
        { 
            get => _hasItems; 
            set => SetProperty(ref _hasItems, value); 
        }

        public bool IsOldData
        {
            get => _isOldData;
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
            RefreshListCommand = new Command<bool>(async (forceRefresh) => await RefreshSensorList(forceRefresh));            
            SelectItemCommand = new Command(() =>
            {                
                if (SelectedMeasurement != null)
                {
                    var location = Locations.FirstOrDefault(l => l.Contains(SelectedMeasurement));
                    NavigationService.NavigateTo(Constants.PageType.Location, location);                    
                }                
            });

            TapLocationCommand = new Command<string>(async (locationName) =>
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

            NoDataMessage = null;
            var updatedNeeded = Locations.Count == 0;

            foreach (var location in Locations)
            {
                if (location != null && (location.Timestamp + TimeSpan.FromMinutes(location.UpdateInterval)) < DateTime.Now)
                    updatedNeeded = true;
            }

            if (IsBusy || (!forceRefresh && !updatedNeeded))
                return;

            IsBusy = true;

            try
            {                
                Locations.Clear();

                IEnumerable<Models.Location> items = new List<Models.Location>();
                try
                {
                    items = await _locationDataService.GetAllDataAsync(forceRefresh || updatedNeeded);
                                        
                    foreach (var location in items)
                    {
                        if (location!= null && (location.Timestamp + TimeSpan.FromMinutes(location.UpdateInterval * 4)) < DateTime.Now)
                        {
                            LastUpdated = string.Format(Resources.AppResources.Locations_LastUpdatedText, $"{location.Timestamp.ToShortDateString()} {location.Timestamp.ToLongTimeString()}");
                            IsOldData = true;
                            break;
                        }
                        else
                        {
                            IsOldData = false;
                        }
                    }                    

                    foreach (var location in items)
                    {
                        var filteredLocation = new Models.Location(location.Name, location.Timestamp, location.UpdateInterval, location.ToList());
                        filteredLocation.RemoveAll(m => !m.IsVisibleInSummaryView);                        
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
                IsBusy = false;
            }
        }        
    }
}