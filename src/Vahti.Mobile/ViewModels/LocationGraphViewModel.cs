using System.Diagnostics;
using Vahti.Mobile.Exceptions;
using Vahti.Mobile.Models;
using Vahti.Mobile.Services;
using System.Collections.ObjectModel;
using MvvmHelpers.Interfaces;
using System.Windows.Input;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using NavigatedToEventArgs = Vahti.Mobile.EventArguments.NavigatedToEventArgs;

namespace Vahti.Mobile.ViewModels
{
    /// <summary>
    /// View model for page displaying sensor history graphs
    /// </summary>
    public class LocationGraphViewModel : BaseViewModel
    {
        private readonly IDataService<MeasurementHistory> _historyDataService;
        private readonly IOptionService _optionService;
        private readonly IGraphService _graphService;
        private ObservableCollection<ChartModel> _charts = new ObservableCollection<ChartModel>();
        private Models.Location _selectedLocation;
        private bool _showGraphs = false;
        private bool _preventUpdate = false;
        
        public IAsyncCommand<bool> RefreshGraphCommand { get; }
        public ICommand ShowDetailsCommand { get; set; }

        public ObservableCollection<ChartModel> Charts
        {
            get
            {
                return _charts;
            }
            set
            {
                SetProperty(ref _charts, value);
            }
        }

        public Models.Location SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                SetProperty(ref _selectedLocation, value);
            }
        }

        public bool ShowGraphs
        {
            get 
            { 
                return _showGraphs; 
            }
            set 
            { 
                SetProperty(ref _showGraphs, value); 
            }
        }    

        public LocationGraphViewModel(IDataService<MeasurementHistory> dataStore, INavigationService navigationService,
            IOptionService optionService, IGraphService graphService) : base(navigationService)
        {
            _historyDataService = dataStore;
            _optionService = optionService;
            _graphService = graphService;

            NavigationService.NavigatedTo += NavigationService_NavigatedTo;
            RefreshGraphCommand = new AsyncCommand<bool>(async (forceRefresh) => await RefreshDataAsync(forceRefresh));
            ShowDetailsCommand = new Command(() =>
            {   
                NavigationService.NavigateTo(Constants.PageType.Details, SelectedLocation);                
            });

            Title = Resources.AppResources.Graph_Title;
        }

        private async void NavigationService_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            if (e.Page == Constants.PageType.Location)
            {
                SelectedLocation = e.Parameter as Models.Location;
                await RefreshDataAsync(true);
            }
        }

        public async Task RefreshDataAsync(bool forceRefresh)
        {
            if (IsBusy || (!forceRefresh && Charts.Count > 0))
                return;

            // Setting IsBusy trigs refresh command (through RefreshView), and this ugly thing prevents the duplicate call
            if (_preventUpdate)
            {
                return;
            }

            IsBusy = _preventUpdate = true;

            try
            {
                Charts.Clear();                
                IReadOnlyList<MeasurementHistory> history = null;

                try
                {
                    history = await _historyDataService.GetAllDataAsync(forceRefresh);
                }
                catch (DataNotFoundException)
                {
                    // TODO: if data id is empty, show message to user
                }

                foreach (var measurement in SelectedLocation)
                {
                    var historyItem = history.FirstOrDefault(h => h.SensorDeviceId.Equals(measurement.SensorDeviceId, StringComparison.OrdinalIgnoreCase) && h.SensorId.Equals(measurement.SensorId));                    
                    if (historyItem == null)
                    {
                        continue;
                    }

                    Charts.Add(_graphService.GetChart(historyItem, measurement.SensorClass, measurement.SensorName, _optionService.ShowMinMaxValues));
                }
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
    }    
}
