using MobileClient.ViewModels;
using Vahti.Mobile.Forms.ViewModels;

namespace Vahti.Mobile.Forms.Views
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