using MobileClient.ViewModels;

namespace Vahti.Mobile.Forms.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();            
            BindingContext = ViewModelLocator.About;
        }
    }
}