using Vahti.Mobile.Resources;
using Vahti.Mobile.Services;
using System.Collections.ObjectModel;
using MvvmHelpers.Interfaces;
using System.Windows.Input;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using Location = Vahti.Mobile.Models.Location;

namespace Vahti.Mobile.ViewModels
{
    /// <summary>
    /// View model for page used to choose what data to show in summary UI
    /// </summary>
    public class OptionsSortingViewModel : BaseViewModel
    {
        private readonly IDataService<Models.Location> _dataService;                
        private bool _locationsSorted;
        private ObservableCollection<Models.Location> _locationList = new ObservableCollection<Models.Location>();

        public ICommand InitializeCommand { get; set; }
        public IAsyncCommand RefreshCommand { get; set; }
        public IAsyncCommand UpdateCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }

        public ObservableCollection<Models.Location> Locations
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

        public OptionsSortingViewModel(INavigationService navigationService, IDataService<Models.Location> dataService) : base(navigationService)
        {
            _dataService = dataService;            

            InitializeCommand = new Command(() => 
            {
                IsBusy = true;
                _locationsSorted = false; 
            });

            RefreshCommand = new AsyncCommand(async () => await RefreshDataAsync());
            UpdateCommand = new AsyncCommand(async () =>
            {
                if (_locationsSorted)
                {
                    foreach (var location in Locations)
                    {
                        await _dataService.UpdateAsync(location);                        
                    }
                    _locationsSorted = false;
                }               
            });

            MoveDownCommand = new Command((item) =>
            {
                MoveItemInList(item as Location, MoveDirection.Down);
            });

            MoveUpCommand = new Command((item) =>
            {
                MoveItemInList(item as Location, MoveDirection.Up);
            });

            Title = AppResources.Options_Title;
        }       

        public async Task RefreshDataAsync()
        {
            try
            {
                Locations.Clear();
                var tempCollection = new ObservableCollection<Models.Location>();
                foreach (var location in await _dataService.GetAllDataAsync(false))
                {
                    //await Task.Delay(50);
                    tempCollection.Add(location);
                }
                Locations = tempCollection;
            }
            finally
            {
                IsBusy = false;
            }            
        }
        
        private void MoveItemInList(Location locationToMove, MoveDirection moveDirection)
        {            
            var originalIndex = Locations.IndexOf(locationToMove);
            int newIndex;
            if (moveDirection == MoveDirection.Up)
            {
                if (originalIndex == 0)
                {
                    return;
                }

                newIndex = originalIndex - 1;
            }
            else 
            {
                if (originalIndex == Locations.Count - 1)
                {
                    return;
                }
                    
                newIndex = originalIndex + 1;
            }            

            
            Locations.Remove(locationToMove);
            Locations.Insert(newIndex, locationToMove);

            foreach (var location in Locations)
            {
                location.Order = Locations.IndexOf(location);
            }

            _locationsSorted = true;
        }

        private enum MoveDirection
        {
            Up, 
            Down
        }
    }
}
