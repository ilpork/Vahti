using Android.App;
using Android.Content;
using Android.Util;
using AndroidX.Core.App;
using Firebase.Messaging;
using Microsoft.Extensions.Configuration;
using Vahti.Mobile.Forms;
using Vahti.Mobile.Forms.Models;
using WindowsAzure.Messaging;

namespace Vahti.Mobile.Droid
{
    /// <summary>
    /// Provides support for handling push notifications
    /// </summary>
    [Service(Exported = false)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class VahtiFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "VahtiFirebaseMsgService";
        NotificationHub hub;

        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);
            if (message.GetNotification() != null)
            {
                //These is how most messages will be received
                Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
                SendNotification("title", message.GetNotification().Body);
            }
            else
            {
                //Only used for debugging payloads sent from the Azure portal
                SendNotification(message.Data["Title"], message.Data["Message"]);

            }
        }

        void SendNotification(string title, string message)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
                        
            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID);
            Context appContext = Android.App.Application.Context;
            int drawableResourceId = appContext.Resources.GetIdentifier("ic_stat_lighthouse", "drawable", appContext.PackageName);
            //var drawable = ContextCompat.GetDrawable(appContext, drawableResourceId);

            notificationBuilder.SetContentTitle(title)
                        .SetSmallIcon(drawableResourceId)
                        .SetContentText(message)
                        .SetAutoCancel(true)
                        .SetShowWhen(false)
                        .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);

            notificationManager.Notify(0, notificationBuilder.Build());
        }

        public override void OnNewToken(string token)
        {
            Log.Debug(TAG, "FCM token: " + token);
            SendRegistrationToServer(token);
        }

        void SendRegistrationToServer(string token)
        {            
            var configuration = MauiProgram.Services.GetService<IConfiguration>();
            var settings = configuration.GetRequiredSection("Settings").Get<AppSettings>();
            var connectionString = settings.AzureListConnectionString;
            var notificationHubName = settings.AzureNotificationHubName;

            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(notificationHubName))
            {
                // Register with Notification Hubs
                hub = new NotificationHub(notificationHubName, connectionString, this);

                var tags = new List<string>() { };
                var regID = hub.Register(token, tags.ToArray()).RegistrationId;

                Log.Debug(TAG, $"Successful registration of ID {regID}");
            }
        }
    }
}