using MobileClient.ViewModels;

namespace Vahti.Mobile.Views
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