using MobileClient.ViewModels;
using System.Runtime.Versioning;
using Vahti.Mobile.EventArguments;
using Vahti.Mobile.ViewModels;

namespace Vahti.Mobile.Views
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class LocationListPage : ContentPage
    {
        LocationListViewModel _viewModel;
        public int MyProperty { get; set; }
        public LocationListPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = ViewModelLocator.Items;
            App.AppResumed += App_AppResumed;
        }

        private void App_AppResumed(object sender, AppResumedEventArgs e)
        {
            if (this == e.CurrentPage)
            {
                _viewModel.RefreshListCommand.Execute(false);
            }
        }

        private void LocationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LocationList.SelectedItem = null;            
        }
    }
}