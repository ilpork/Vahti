using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using PCLAppConfig;
using Vahti.Mobile.Forms.Localization;
using Vahti.Mobile.Forms.Models;
using Vahti.Mobile.Forms.Services;

namespace Vahti.Mobile.Droid
{
    /// <summary>
    /// Show selected measurement values in home screen widget
    /// </summary>
    [BroadcastReceiver (Label = "@string/data_widget_name", Exported = false)]
	[IntentFilter (new string [] { "android.appwidget.action.APPWIDGET_UPDATE" })]
	[MetaData ("android.appwidget.provider", Resource = "@xml/data_widget")]    
    public class DataWidget : AppWidgetProvider
    {
        private readonly IDataService<Forms.Models.Location> _locationDataService;

        public DataWidget()
        {
            if (ConfigurationManager.AppSettings == null)
            {
                ConfigurationManager.Initialise(PCLAppConfig.FileSystemStream.PortableStream.Current);
            }

            _locationDataService = CommonServiceLocator.ServiceLocator.Current.GetInstance<IDataService<Forms.Models.Location>>();            
        }

        public async override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(DataWidget)).Name);
            var remoteViews = await BuildRemoteViews(context, appWidgetIds);

            if (remoteViews != null)
            {
                appWidgetManager.UpdateAppWidget(me, remoteViews);
            }            
        }

        private async Task<RemoteViews> BuildRemoteViews(Context context, int[] appWidgetIds)
        {
            RemoteViews widgetView = new RemoteViews(context.PackageName, Resource.Layout.widget_measurements);
            IList<Location> locationList = null;

            try
            {
                locationList = (await _locationDataService.GetAllDataAsync(true)).ToList();
            }
            catch
            {
                // Error handling needs more work, and check how to make widget work more reliably in general
                // For now, just go without updating anything on widget if reading data fails due to OS preventing access etc.
            }

            if (locationList != null)
            {
                if (locationList.Count > 0)
                {
                    var measurements = new List<Tuple<string, Measurement>>();
                    foreach (var location in locationList)
                    {
                        foreach (var measurement in location)
                        {
                            if (measurement.IsVisibleInWidget)
                            {
                                measurements.Add(new Tuple<string, Measurement>(location.Name, measurement));
                            }
                        }
                    }

                    if (measurements.Count >= 1)
                    {
                        widgetView.SetTextViewText(Resource.Id.location1, measurements[0].Item1);
                        widgetView.SetTextViewText(Resource.Id.sensor1, measurements[0].Item2.SensorName);
                        widgetView.SetTextViewText(Resource.Id.value1, DisplayValueFormatter.GetMeasurementDisplayValue(
                            measurements[0].Item2.SensorClass, measurements[0].Item2.Value, measurements[0].Item2.Unit));
                    }
                    if (measurements.Count >= 2)
                    {
                        widgetView.SetTextViewText(Resource.Id.location2, measurements[1].Item1);
                        widgetView.SetTextViewText(Resource.Id.sensor2, measurements[1].Item2.SensorName);
                        widgetView.SetTextViewText(Resource.Id.value2, DisplayValueFormatter.GetMeasurementDisplayValue(
                            measurements[1].Item2.SensorClass, measurements[1].Item2.Value, measurements[1].Item2.Unit));
                    }
                    if (measurements.Count >= 3)
                    {
                        widgetView.SetTextViewText(Resource.Id.location3, measurements[2].Item1);
                        widgetView.SetTextViewText(Resource.Id.sensor3, measurements[2].Item2.SensorName);
                        widgetView.SetTextViewText(Resource.Id.value3, DisplayValueFormatter.GetMeasurementDisplayValue(
                            measurements[2].Item2.SensorClass, measurements[2].Item2.Value, measurements[2].Item2.Unit));
                    }
                    var firstLocation = locationList.FirstOrDefault();
                    widgetView.SetTextViewText(Resource.Id.updated, string.Format("{0}:{1}", firstLocation.Timestamp.Hour, firstLocation.Timestamp.Minute.ToString("00")));
                }
                else
                {
                    widgetView.SetTextViewText(Resource.Id.updated, context.Resources.GetString(Resource.String.no_widget_data_found));
                }
            }                        

            RegisterClicks(context, appWidgetIds, widgetView);

            return widgetView;
        }
        
        private void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
        {
            var intent = new Intent(context, typeof(DataWidget));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);

            var pendingIntent = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            widgetView.SetOnClickPendingIntent(Resource.Id.widget, pendingIntent);
        }
    }
}
