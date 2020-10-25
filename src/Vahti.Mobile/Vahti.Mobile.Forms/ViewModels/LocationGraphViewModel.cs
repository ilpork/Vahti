using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OxyPlot;
using Vahti.Mobile.Forms.Exceptions;
using Vahti.Mobile.Forms.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Vahti.Mobile.Forms.EventArguments;
using Vahti.Mobile.Forms.Services;
using System.Collections.ObjectModel;

namespace Vahti.Mobile.Forms.ViewModels
{
    /// <summary>
    /// View model for page displaying sensor history graphs
    /// </summary>
    public class LocationGraphViewModel : BaseViewModel
    {
        private readonly IDataService<MeasurementHistory> _historyDataService;
        private ObservableCollection<IPlotModel> _plotModels = new ObservableCollection<IPlotModel>();
        private Models.Location _selectedLocation;
        private bool _showGraphs = false;
        
        public Command<bool> RefreshGraphCommand { get; }
        
        
        public ObservableCollection<IPlotModel> PlotModels
        {
            get
            {
                return _plotModels;
            }
            set
            {
                SetProperty(ref _plotModels, value);
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

        public LocationGraphViewModel(IDataService<MeasurementHistory> dataStore, INavigationService navigationService) : base(navigationService)
        {
            _historyDataService = dataStore;
            NavigationService.NavigatedTo += NavigationService_NavigatedTo;
            RefreshGraphCommand = new Command<bool>(async (forceRefresh) => await RefreshDataAsync(forceRefresh));
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
            if (IsBusy || (!forceRefresh && PlotModels.Count > 0))
                return;

            IsBusy = true;

            try
            {
                PlotModels.Clear();                
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

                    PlotModels.Add(GraphModel.GetPlotModel(historyItem, measurement.SensorClass, measurement.SensorName));
                }
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
