using Vahti.Mobile.Forms.Views;

namespace Vahti.Mobile.Forms
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
