﻿using MobileClient.ViewModels;
using Vahti.Mobile.Forms.ViewModels;
using Xamarin.Forms;

namespace Vahti.Mobile.Forms.Views
{
    public partial class LocationGraphPage : ContentPage
    {
        private readonly LocationGraphViewModel vm;

        public LocationGraphPage()
        {
            InitializeComponent();

            BindingContext = vm = ViewModelLocator.Graph;
        }        
    }
}