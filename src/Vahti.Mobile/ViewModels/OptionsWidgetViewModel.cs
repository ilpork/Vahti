﻿using Vahti.Mobile.Resources;
using Vahti.Mobile.Services;
using System.Windows.Input;
using MvvmHelpers.Interfaces;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;

namespace Vahti.Mobile.ViewModels
{
    /// <summary>
    /// View model for page used to choose what data to show in app widget
    /// </summary>
    public class OptionsWidgetViewModel : BaseViewModel
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

        public OptionsWidgetViewModel(INavigationService navigationService, IDataService<Models.Location> dataService) : base(navigationService)
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
