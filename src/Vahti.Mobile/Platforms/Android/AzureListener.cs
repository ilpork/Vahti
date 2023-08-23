using Android.App;
using Android.Content;
using AndroidX.Core.App;
using WindowsAzure.Messaging.NotificationHubs;

namespace Vahti.Mobile.Droid
{
    public class AzureListener : Java.Lang.Object, INotificationListener
    {
        public void OnPushNotificationReceived(Context context, INotificationMessage message)
        {
            var intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

            var notificationBuilder = new NotificationCompat.Builder(context, MainActivity.CHANNEL_ID);            
            int drawableResourceId = context.Resources.GetIdentifier("ic_stat_lighthouse", "drawable", context.PackageName);

            string title;
            string messageBody;

            // Payload sent by Vahti.DataBroker
            if (message.Data != null && message.Data.ContainsKey("Title") && message.Data.ContainsKey("Message"))
            {
                title = message.Data["Title"];
                messageBody = message.Data["Message"];
            }
            else
            {
                title = message.Title;
                messageBody = message.Body;
            }

            notificationBuilder.SetContentTitle(title)
                        .SetSmallIcon(drawableResourceId)
                        .SetContentText(messageBody)
                        .SetAutoCancel(true)
                        .SetShowWhen(false)
                        .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(context);

            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}
