using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Vahti.Mobile.Forms.Models;
using Vahti.Mobile.Forms.Services;

namespace Vahti.Mobile.Droid
{
    /// <summary>
    /// Old homescreen widget experiment with fixed data. It needs be updated to work with current implementation
    /// </summary>
	//[BroadcastReceiver (Label = "@string/widget_name")]
	//[IntentFilter (new string [] { "android.appwidget.action.APPWIDGET_UPDATE" })]
	//[MetaData ("android.appwidget.provider", Resource = "@xml/widget_sensor")]
 //   public class MeasurementWidget : AppWidgetProvider
 //   {
 //       public async override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
 //       {
 //           var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(SensorWidget)).Name);
 //           var remoteViews = await BuildRemoteViews(context, appWidgetIds);

 //           if (remoteViews != null)
 //           {
 //               appWidgetManager.UpdateAppWidget(me, remoteViews);
 //           }
 //       }

 //       private async Task<RemoteViews> BuildRemoteViews(Context context, int[] appWidgetIds)
 //       {
 //           RemoteViews widgetView;
 //           var ds = new SensorDataService(new FirebaseDataProvider());
 //           IEnumerable<Sensor> latestData;

 //           try
 //           {
 //               latestData = await ds.GetLatestDataAsync("ompsu");
 //           }
 //           catch (Exception)
 //           {
 //               return null;
 //           }

 //           widgetView = new RemoteViews(context.PackageName, Resource.Layout.widget_sensors);

 //           var sensor1 = latestData.FirstOrDefault(s => s.Name.Equals("location1")) as Ruuvi;
 //           var sensor2 = latestData.FirstOrDefault(s => s.Name.Equals("location2")) as Ruuvi;
 //           var sensor3 = latestData.FirstOrDefault(s => s.Name.Equals("location3")) as Ruuvi;
 //           widgetView.SetTextViewText(Resource.Id.header1, sensor1.Name);
 //           widgetView.SetTextViewText(Resource.Id.value1, (sensor1.Temperature < 0) ? sensor1.Temperature.ToString("0.0") + "°C" : string.Format("+{0:0.0}°C", sensor1.Temperature));
 //           widgetView.SetTextViewText(Resource.Id.header2, sensor2.Name);
 //           widgetView.SetTextViewText(Resource.Id.value2, (sensor2.Temperature < 0) ? sensor2.Temperature.ToString("0.0") + "°C" : string.Format("+{0:0.0}°C", sensor2.Temperature));
 //           widgetView.SetTextViewText(Resource.Id.header3, sensor3.Name);
 //           widgetView.SetTextViewText(Resource.Id.value3, (sensor3.Temperature < 0) ? sensor3.Temperature.ToString("0.0") + "°C" : string.Format("+{0:0.0}°C", sensor3.Temperature));
 //           widgetView.SetTextViewText(Resource.Id.updated, string.Format("{0}:{1}", sensor1.Timestamp.Hour, sensor1.Timestamp.Minute.ToString("00")));

 //           RegisterClicks(context, appWidgetIds, widgetView);

 //           return widgetView;
 //       }

 //       private void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
 //       {
 //           var intent = new Intent(context, typeof(SensorWidget));
 //           intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
 //           intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);

 //           // Register click event for the Background
 //           var piBackground = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
 //           widgetView.SetOnClickPendingIntent(Resource.Id.widget, piBackground);
 //       }
 //   }
}
