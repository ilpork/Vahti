using MobileClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vahti.Mobile.Forms.ViewModels;
using Vahti.Mobile.Forms.Views.Fonts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vahti.Mobile.Forms.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OptionsGeneralPage : ContentPage
    {
        public bool _databaseSecretHidden = false;        

        public OptionsGeneralPage()
        {
            InitializeComponent();

            ToggleDatabaseSecretCharacters();
            BindingContext = ViewModelLocator.OptionsGeneral;
        }

        private void EyeLabel_Tapped(object sender, EventArgs e)
        {
            ToggleDatabaseSecretCharacters();

        }

        private void ToggleDatabaseSecretCharacters()
        {
            if (_databaseSecretHidden)
            {
                _databaseSecretHidden = entryDatabaseSecret.IsPassword = false;
                labelEye.Text = FontIcons.FontAwesomeEyeSlash;
            }
            else
            {
                _databaseSecretHidden = entryDatabaseSecret.IsPassword = true;
                labelEye.Text = FontIcons.FontAwesomeEye;
            }
        }
    }
}