using PCLAppConfig;
using System;
using Vahti.Shared.DataProvider;
using Xamarin.Essentials;

namespace Vahti.Mobile.Forms.Services
{
    /// <summary>
    /// Provides functionality to manage database access
    /// </summary>
    public class DatabaseManagementService : IDatabaseManagementService
    {
        private const string DatabaseUrlKey = "DatabaseUrl";
        private const string DatabaseSecretKey = "DatabaseSecret";
        private readonly IDataProvider _dataProvider;

        public DatabaseManagementService(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public bool HasStaticConfiguration
        {
            get
            {
                var databaseUrl = ConfigurationManager.AppSettings[Constants.AppConfig.FirebaseDatabaseUrl];

                return (!string.IsNullOrEmpty(databaseUrl)) ? true : false;                
            }
        }

        public bool HasDatabaseUrlDefined
        {
            get
            {
                return HasStaticConfiguration || !string.IsNullOrEmpty(DatabaseUrl);
            }
        }

        public string DatabaseUrl
        {
            get
            {
                return Preferences.Get(DatabaseUrlKey, String.Empty);
            }
            set
            {
                Preferences.Set(DatabaseUrlKey, value);
                _dataProvider.SetConfiguration(DatabaseUrl, DatabaseSecret);
            }
        }

        public string DatabaseSecret
        {
            get
            {
                return Preferences.Get(DatabaseSecretKey, String.Empty);
            }
            set
            {
                Preferences.Set(DatabaseSecretKey, value);
                _dataProvider.SetConfiguration(DatabaseUrl, DatabaseSecret);
            }
        }

        public void SetDatabaseConfiguration()
        {
            if (HasStaticConfiguration)
            {
                var databaseUrl = ConfigurationManager.AppSettings[Constants.AppConfig.FirebaseDatabaseUrl];
                var databaseSecret = ConfigurationManager.AppSettings[Constants.AppConfig.FirebaseDatabaseSecret];
                _dataProvider.SetConfiguration(databaseUrl, databaseSecret);
            }
            else
            {
                var databaseUrl = Preferences.Get(DatabaseUrlKey, String.Empty);
                var databaseSecret = Preferences.Get(DatabaseSecretKey, String.Empty);
                _dataProvider.SetConfiguration(databaseUrl, databaseSecret);
            }

        }
    }
}
