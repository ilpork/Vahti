using Vahti.Shared.DataProvider;
using Vahti.Mobile.Forms.Models;

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
        private readonly AppSettings _settings;

        public DatabaseManagementService(IDataProvider dataProvider, AppSettings settings)
        {
            _dataProvider = dataProvider;
            _settings = settings;
        }

        public bool HasStaticConfiguration
        {
            get
            {
                var databaseUrl = _settings.FirebaseDatabaseUrl;

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
                var databaseUrl = _settings.FirebaseDatabaseUrl;
                var databaseSecret = _settings.FirebaseDatabaseSecret;
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
