using MobileClient.ViewModels;
using Vahti.Mobile.ViewModels;

namespace Vahti.Mobile.Views
{
    public partial class LocationGraphPage : ContentPage
    {
        private readonly LocationGraphViewModel vm;

        public LocationGraphPage()
        {
            InitializeComponent();

            BindingContext = vm = ViewModelLocator.Graph;
        }        
    }
}