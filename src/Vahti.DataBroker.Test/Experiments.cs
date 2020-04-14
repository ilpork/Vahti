using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vahti.DataBroker.Configuration;
using Vahti.DataBroker.DataProvider;
using Vahti.DataBroker.Notifier;
using Vahti.Shared.Data;
using Vahti.Shared.DataProvider;

namespace Vahti.Server.Test
{
    /// <summary>
    /// Experimental testing of implemented functionality. Ignored when running the tests
    /// </summary>
    [TestClass]    
    public class Experiments
    {
        // Commented out because Azure DevOps lower test success percentage if there are ignored tests 
        //[TestMethod]
        //[Ignore]
        public async Task GroupHistoryDataPerformanceExperiment()
        {
            var deviceId = "ruuvi1";
            var sensorId = "temperature";

            var db = new SqLiteHistoryDataProvider();
            await db.Flush();
            var dtNow = DateTime.Now;
            var sw = new Stopwatch();
            sw.Start();
            var list = new List<MeasurementData>();

            for (int i = 800; i > 0; i--)
            {
                var data = new MeasurementData() { Timestamp = new DateTime(dtNow.Ticks - TimeSpan.TicksPerMinute * i * 5), SensorDeviceId = deviceId, SensorId = sensorId, Value = i.ToString() };
                list.Add(data);
            }
            await db.AddItems(list);
            sw.Stop();

            sw.Start();
            var all = await db.GetHistory(deviceId, sensorId, 10);
            sw.Stop();
            sw.Start();

            var groups = from s in all
                         let groupKey = new DateTime(s.Timestamp.Year, s.Timestamp.Month, s.Timestamp.Day, s.Timestamp.Hour, 0, 0)
                         group s by groupKey into g
                         select new HistoryValueData
                         {
                             Timestamp = g.Key,
                             Value = g.First().Value
                         };
            var x = groups.ToList();
            sw.Stop();
        }

        // Commented out because Azure DevOps lower test success percentage if there are ignored tests
        //[TestMethod]
        //[Ignore]
        public async Task SendNotificationTestAzureAndEmail()
        {
            var devices = new List<MobileDeviceData>() { new MobileDeviceData() { Id = "deviceId", Name = "S1x" } };
            var dbMock = new Mock<IDataProvider>();
            var configMock = new Mock<IOptions<DataBrokerConfiguration>>();
            dbMock.Setup(s => s.LoadAllItemsAsync<MobileDeviceData>()).ReturnsAsync(devices);
            configMock.Setup(s => s.Value.AlertConfiguration.AzurePushNotifications.Enabled).Returns(false);
            configMock.Setup(s => s.Value.AlertConfiguration.AzurePushNotifications.ConnectionString).Returns("");
            configMock.Setup(s => s.Value.AlertConfiguration.AzurePushNotifications.NotificationHubName).Returns("Vahti");
            configMock.Setup(s => s.Value.AlertConfiguration.EmailNotifications.Enabled).Returns(true);
            configMock.Setup(s => s.Value.AlertConfiguration.EmailNotifications.Recipients).Returns(new List<string>() { "" });
            configMock.Setup(s => s.Value.AlertConfiguration.EmailNotifications.Sender).Returns("");
            configMock.Setup(s => s.Value.AlertConfiguration.EmailNotifications.SmtpServerAddress).Returns("");
            configMock.Setup(s => s.Value.AlertConfiguration.EmailNotifications.SmtpServerPort).Returns(25);
            configMock.Setup(s => s.Value.AlertConfiguration.EmailNotifications.SmtpAuthentication.Enabled).Returns(false);
            configMock.Setup(s => s.Value.AlertConfiguration.EmailNotifications.SmtpAuthentication.UserName).Returns("sfsdfo");
            configMock.Setup(s => s.Value.AlertConfiguration.EmailNotifications.SmtpAuthentication.Password).Returns("sdfds");
            var s = new NotificationService(configMock.Object);
            await s.Send("Alert", "Something has happened");
        }
    }
}
