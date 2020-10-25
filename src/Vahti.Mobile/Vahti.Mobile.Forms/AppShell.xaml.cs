﻿using System;
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

            Routing.RegisterRoute("summary", typeof(LocationListPage));
            Routing.RegisterRoute("location", typeof(LocationPage));
        }
    }
}
