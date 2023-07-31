using MobileClient.ViewModels;
using Vahti.Mobile.Views.Fonts;

namespace Vahti.Mobile.Views
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