using BleReaderNet.Reader;
using BleReaderNet.Wrapper;
using BleReaderNet.Wrapper.DotNetBlueZ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vahti.BluetoothGw;
using Vahti.BluetoothGw.Configuration;
using Vahti.BluetoothGw.DeviceScanner;
using Vahti.DataBroker;
using Vahti.DataBroker.AlertHandler;
using Vahti.DataBroker.Configuration;
using Vahti.DataBroker.DataBroker;
using Vahti.DataBroker.DataProvider;
using Vahti.DataBroker.Notifier;
using Vahti.Mqtt;
using Vahti.Mqtt.Configuration;
using Vahti.Shared.Configuration;
using Vahti.Shared.DataProvider;
using Vahti.Shared.TypeData;

namespace Vahti.Server
{
    class Program
    {
        private const string configurationFileName = "config.json";

        static async Task Main(string[] args)
        {
            var builder = new HostBuilder().ConfigureAppConfiguration((hostingContext, config) =>
            {
                if (args != null)
                {
                    config.AddJsonFile(configurationFileName);
                    config.AddCommandLine(args);
                }
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOptions();

                var firebaseConfiguration = hostContext.Configuration.GetSection("dataBrokerConfiguration").GetSection("cloudPublishConfiguration").GetSection("firebaseStorage");

                services.Configure<MqttConfiguration>(hostContext.Configuration.GetSection("mqttConfiguration"));
                services.AddHostedService<MqttService>();

                services.Configure<DataBrokerConfiguration>(hostContext.Configuration.GetSection("dataBrokerConfiguration"));
                services.Configure<FirebaseConfiguration>(firebaseConfiguration);
                services.AddSingleton<IDataProvider, FirebaseDataProvider>();
                services.AddSingleton<IHistoryDataProvider, SqLiteHistoryDataProvider>();
                services.AddSingleton<INotificationService, NotificationService>();
                services.AddSingleton<INoticationDataProvider, SqLiteNotificationDataProvider>();
                services.AddSingleton<IDataBroker, DataBroker.DataBroker.DataBroker>();
                services.AddSingleton<IAlertHandler, AlertHandler>();
                services.AddHostedService<DataBrokerService>();

                services.Configure<BluetoothGwConfiguration>(hostContext.Configuration.GetSection("bluetoothGwConfiguration"));
                services.Configure<List<SensorDevice>>(hostContext.Configuration.GetSection("sensorDevice"));
                services.Configure<List<SensorDeviceType>>(hostContext.Configuration.GetSection("sensorDevicetype"));
                services.AddSingleton<IBluetoothService, DotNetBlueZService>();
                services.AddSingleton<IBleReader, BleReader>();
                services.AddSingleton<IDeviceScanner, DeviceScanner>();
                services.AddTransient((p) => new MqttFactory().CreateManagedMqttClient());
                services.AddHostedService<BluetoothGwService>();

            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole((options) =>
                {
                    options.Format = Microsoft.Extensions.Logging.Console.ConsoleLoggerFormat.Default;
                });
            });

            await builder.RunConsoleAsync();
        }
    }
}

