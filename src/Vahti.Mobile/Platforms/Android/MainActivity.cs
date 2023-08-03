using Android.Util;
using Android.Gms.Common;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Firebase;
using Android;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Android.Runtime;
using WindowsAzure.Messaging.NotificationHubs;
using Vahti.Mobile;
using Microsoft.Extensions.Configuration;
using Vahti.Mobile.Models;

namespace Vahti.Mobile.Droid
{
    [Activity(Label = "Vahti", Theme = "@style/LightTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Microsoft.Maui.MauiAppCompatActivity
    {        
        public const string TAG = "MainActivity";
        internal static readonly string CHANNEL_ID = "my_notification_channel";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    if (key != null)
                    {
                        var value = Intent.Extras.GetString(key);
                        Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                    }
                }
            }

            IsPlayServicesAvailable();
            CreateNotificationChannel();

            FirebaseApp.InitializeApp(this);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.PostNotifications) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.PostNotifications }, 0);
            }

            // Listen for push notifications
            NotificationHub.SetListener(new AzureListener());

            // Start the SDK
            var configuration = MauiProgram.Services.GetService<IConfiguration>();
            var settings = configuration.GetRequiredSection("Settings").Get<AppSettings>();
            var connectionString = settings.AzureListConnectionString;
            var notificationHubName = settings.AzureNotificationHubName;
            
            if (notificationHubName != null && connectionString != null)
                NotificationHub.Start(Application, notificationHubName, connectionString);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    Log.Debug(TAG, GoogleApiAvailability.Instance.GetErrorString(resultCode));
                else
                {
                    Log.Debug(TAG, "This device is not supported");
                    Finish();
                }
                return false;
            }

            Log.Debug(TAG, "Google Play Services is available.");
            return true;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID, CHANNEL_ID, NotificationImportance.Default)
            {
                Description = string.Empty
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }    
}