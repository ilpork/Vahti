using MobileClient.ViewModels;

namespace Vahti.Mobile.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OptionsSummaryPage : ContentPage
    {
        public OptionsSummaryPage()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.OptionsSummary;
        }
    }
}