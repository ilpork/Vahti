using MobileClient.ViewModels;
using Vahti.Mobile.ViewModels;

namespace Vahti.Mobile.Views
{
    public partial class LocationDetailsPage : ContentPage
    {
        private readonly LocationDetailsViewModel vm;

        public LocationDetailsPage()
        {
            InitializeComponent();

            BindingContext = vm = ViewModelLocator.Details;
        }

        public async Task RefreshGraph()
        {
            await vm.RefreshDataAsync();
        }

    }
}