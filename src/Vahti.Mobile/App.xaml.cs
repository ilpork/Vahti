﻿using Autofac;
using Vahti.Mobile.Models;
using Vahti.Mobile.ViewModels;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Vahti.Mobile.Theme;
using Vahti.Mobile.Localization;
using Vahti.Mobile.Resources;
using Vahti.Mobile.Services;
using Vahti.Shared.DataProvider;
using Vahti.Shared.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Vahti.Mobile.EventArguments;
using System.Runtime.Versioning;
#if ANDROID
using Android.Content.Res;
#endif
#if ANDROID
using Android.App;
using Android.Content;
using Android.Views;
using System.Diagnostics;
#endif

namespace Vahti.Mobile
{
    [SupportedOSPlatform("windows10.0.17763.0")]
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public static event EventHandler<AppResumedEventArgs> AppResumed;

        public App()
        {
            InitializeComponent();
                 
            ColorThemeChanger.ApplyTheme();
                        
            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.Android)
            {
                var localize = new Localize();
                var ci = localize.GetCurrentCultureInfo();
                AppResources.Culture = ci;
                localize.SetLocale(ci);
            }
            
            var containerBuilder = new ContainerBuilder();
            var configuration = MauiProgram.Services.GetService<IConfiguration>();
            var settings = configuration.GetRequiredSection("Settings").Get<AppSettings>();
            containerBuilder.RegisterInstance(settings).As(typeof(AppSettings)).SingleInstance();
            // Register data provider. Mock data provider is used in DebugMock build configuration
#if DEBUGMOCK
            containerBuilder.RegisterType<MockDataProvider>().As(typeof(IDataProvider)).SingleInstance();
#else            
            var firebaseConfigurationOptions = Options.Create<FirebaseConfiguration>(new FirebaseConfiguration() 
                { Enabled = true });

            containerBuilder.RegisterInstance(firebaseConfigurationOptions).As(typeof(IOptions<FirebaseConfiguration>)).SingleInstance();
            containerBuilder.RegisterType<FirebaseDataProvider>().As(typeof(IDataProvider)).SingleInstance();
#endif
            containerBuilder.RegisterType<DatabaseManagementService>().As(typeof(IDatabaseManagementService)).SingleInstance();
            
            // Register services
            containerBuilder.RegisterType<LocationDataService>().As(typeof(IDataService<Models.Location>)).SingleInstance();
            containerBuilder.RegisterType<HistoryDataService>().As(typeof(IDataService<MeasurementHistory>)).SingleInstance();            
            containerBuilder.RegisterType<NavigationService>().As(typeof(INavigationService)).SingleInstance();
            containerBuilder.RegisterType<OptionService>().As(typeof(IOptionService)).SingleInstance();
            containerBuilder.RegisterType<GraphService>().As(typeof(IGraphService)).SingleInstance();

            // Register view models
            containerBuilder.RegisterType<LocationListViewModel>().As(typeof(LocationListViewModel)).SingleInstance().AutoActivate();            
            containerBuilder.RegisterType<LocationGraphViewModel>().As(typeof(LocationGraphViewModel)).SingleInstance().AutoActivate();
            containerBuilder.RegisterType<LocationDetailsViewModel>().As(typeof(LocationDetailsViewModel)).SingleInstance().AutoActivate();
            containerBuilder.RegisterType<OptionsGeneralViewModel>().As(typeof(OptionsGeneralViewModel)).SingleInstance();
            containerBuilder.RegisterType<OptionsSummaryViewModel>().As(typeof(OptionsSummaryViewModel)).SingleInstance();
            containerBuilder.RegisterType<OptionsWidgetViewModel>().As(typeof(OptionsWidgetViewModel)).SingleInstance();
            containerBuilder.RegisterType<OptionsSortingViewModel>().As(typeof(OptionsSortingViewModel)).SingleInstance();
            containerBuilder.RegisterType<AboutViewModel>().As(typeof(AboutViewModel)).SingleInstance();

            var container = containerBuilder.Build();
            //container.Resolve<LocationListViewModel>();
            
            var dbManagementService = container.Resolve<IDatabaseManagementService>();
            dbManagementService.SetDatabaseConfiguration();
           
            AutofacServiceLocator asl = new AutofacServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => asl);
            
            MainPage = new AppShell();            
        }            

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            AppResumed?.Invoke(this, new AppResumedEventArgs(Shell.Current.CurrentPage));  
        }


        protected override void OnStart()
        {
            base.OnStart();
            ColorThemeChanger.UpdateStatusBar();
        }
    }
}
