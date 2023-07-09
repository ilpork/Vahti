using MobileClient.ViewModels;

namespace Vahti.Mobile.Forms.Views
{
    public partial class LocationListPage : ContentPage
    {
        public int MyProperty { get; set; }
        public LocationListPage()
        {
            InitializeComponent();

            BindingContext = ViewModelLocator.Items;
        }

        private void LocationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LocationList.SelectedItem = null;
        }
    }
}