using Vahti.Mobile.Views;

namespace Vahti.Mobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("options", typeof(OptionsGeneralPage));
            Routing.RegisterRoute("location", typeof(LocationGraphPage));
            Routing.RegisterRoute("location/details", typeof(LocationDetailsPage));
        }
    }
}
