using MobileClient.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vahti.Mobile.Forms.Views
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