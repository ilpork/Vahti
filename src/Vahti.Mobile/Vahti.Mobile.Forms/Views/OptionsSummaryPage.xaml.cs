using MobileClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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