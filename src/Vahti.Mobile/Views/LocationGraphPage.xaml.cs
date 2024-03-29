﻿using MobileClient.ViewModels;
using System.Runtime.Versioning;
using Vahti.Mobile.EventArguments;
using Vahti.Mobile.ViewModels;

namespace Vahti.Mobile.Views
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class LocationGraphPage : ContentPage
    {
        private readonly LocationGraphViewModel _vm;

        public LocationGraphPage()
        {
            InitializeComponent();

            BindingContext = _vm = ViewModelLocator.Graph;
            App.AppResumed += App_AppResumed;
        }
        private void App_AppResumed(object sender, AppResumedEventArgs e)
        {
            if (this == e.CurrentPage)
            {
                _vm.RefreshGraphCommand.Execute(true);
            }
        }
    }
}