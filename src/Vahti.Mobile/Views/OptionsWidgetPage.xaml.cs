using MobileClient.ViewModels;

namespace Vahti.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OptionsWidgetPage : ContentPage
    {
        public OptionsWidgetPage()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.OptionsWidget;
        }
    }
}