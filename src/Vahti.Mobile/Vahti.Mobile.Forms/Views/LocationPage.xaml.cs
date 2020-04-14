using MobileClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vahti.Mobile.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationPage : TabbedPage
    {
        public LocationPage()
        {
            InitializeComponent();

            BindingContext = ViewModelLocator.Location;            
        }       

        private void Refresh_Clicked(object sender, System.EventArgs e)
        {
            //if (CurrentPage == graphPage)
            //    await graphPage.RefreshGraph();
            //else
            //    await detailsPage.RefreshGraph();
        }
    }
}