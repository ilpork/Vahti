using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Vahti.Mobile.Forms.Models;
using Vahti.Mobile.Forms.ViewModels;
using MobileClient.ViewModels;
using Vahti.Mobile.Forms.Views.Fonts;

namespace Vahti.Mobile.Forms.Views
{
    public partial class OptionsPage : ContentPage
    {
        public bool _databaseSecretHidden = false;
        OptionsViewModel viewModel;

        public OptionsPage()
        {
            InitializeComponent();

            ToggleDatabaseSecretCharacters();

            BindingContext = this.viewModel = ViewModelLocator.Options;
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