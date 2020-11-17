using System;
using System.Collections.Generic;
using Vahti.Mobile.Forms.Views;
using Xamarin.Forms;

namespace Vahti.Mobile.Forms
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
                        
            Routing.RegisterRoute("location", typeof(LocationGraphPage));
            Routing.RegisterRoute("location/details", typeof(LocationDetailsPage));
        }
    }
}
