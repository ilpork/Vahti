using System;
using Xamarin.Forms;
using Autofac;
using Vahti.Mobile.Forms.Models;
using Vahti.Mobile.Forms.ViewModels;
using Autofac.Extras.CommonServiceLocator;
using CommonServiceLocator;
using Vahti.Mobile.Forms.Theme;
using Vahti.Mobile.Forms.Localization;
using Device = Xamarin.Forms.Device;
using Vahti.Mobile.Forms.Resources;
using System.Diagnostics;
using Vahti.Mobile.Forms.Services;
using Vahti.Shared.Data;
using Vahti.Shared.DataProvider;
using PCLAppConfig;
using Vahti.Shared.Configuration;
using Microsoft.Extensions.Options;
using Xamarin.Essentials;

namespace Vahti.Mobile.Forms
{
    public partial class App : Application
    {
        public ColorTheme Theme { get; }

        public App(IThemeChanger themeChanger)
        {
            InitializeComponent();

            Theme = new ColorTheme(themeChanger); 
            Theme.ApplyColorTheme();

#if DEBUG
            try
            {
                if (!HotReloader.Current.IsRunning)
                {
                    HotReloader.Current.Run(this);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Starting HotReload failed: {e.Message}");
            }            
#endif                 

            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                AppResources.Culture = ci;
                DependencyService.Get<ILocalize>().SetLocale(ci);
            }
            
            var containerBuilder = new ContainerBuilder();

            // Register data provider. Mock data provider is used in DebugMock build configuration
#if DEBUGMOCK
            containerBuilder.RegisterType<MockDataProvider>().As(typeof(IDataProvider)).SingleInstance();
#else            
            var firebaseConfigurationOptions = Options.Create<FirebaseConfiguration>(new FirebaseConfiguration() 
                { Enabled = true });

            containerBuilder.RegisterInstance(firebaseConfigurationOptions).As(typeof(IOptions<FirebaseConfiguration>));
            containerBuilder.RegisterType<FirebaseDataProvider>().As(typeof(IDataProvider)).SingleInstance();
#endif
            containerBuilder.RegisterType<DatabaseManagementService>().As(typeof(IDatabaseManagementService)).SingleInstance();
            
            // Register services
            containerBuilder.RegisterType<LocationDataService>().As(typeof(IDataService<Models.Location>)).SingleInstance();
            containerBuilder.RegisterType<HistoryDataService>().As(typeof(IDataService<MeasurementHistory>)).SingleInstance();            
            containerBuilder.RegisterType<NavigationService>().As(typeof(INavigationService)).SingleInstance();

            // Register view models
            containerBuilder.RegisterType<LocationListViewModel>().As(typeof(LocationListViewModel)).SingleInstance().AutoActivate();
            containerBuilder.RegisterType<LocationViewModel>().As(typeof(LocationViewModel)).SingleInstance().AutoActivate();
            containerBuilder.RegisterType<LocationGraphViewModel>().As(typeof(LocationGraphViewModel)).SingleInstance().AutoActivate();
            containerBuilder.RegisterType<LocationDetailsViewModel>().As(typeof(LocationDetailsViewModel)).SingleInstance().AutoActivate();
            containerBuilder.RegisterType<OptionsViewModel>().As(typeof(OptionsViewModel)).SingleInstance();
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
            // Handle when your app resumes
        }
    }
}
