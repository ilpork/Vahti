using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vahti.DataBroker.Configuration;
using Vahti.DataBroker.DataBroker;

namespace Vahti.DataBroker.Test
{
    /// <Summary>
    /// Unit tests for <see cref="DataBrokerService"/>
    /// </Summary>
    [TestClass]
    public class DataBrokerServiceTests
    {
        private Mock<ILogger<DataBrokerService>> _loggerMock;
        private Mock<IOptions<DataBrokerConfiguration>> _configMock;
        private Mock<IManagedMqttClient> _mqttClientMock;
        private Mock<IDataBroker> _dataBrokerMock;

        private ServiceProvider _serviceProvider;

        [TestInitialize]
        public void InitializeTest()
        {
            _loggerMock = new Mock<ILogger<DataBrokerService>>();
            _configMock = new Mock<IOptions<DataBrokerConfiguration>>();
            _mqttClientMock = new Mock<IManagedMqttClient>();
            _dataBrokerMock = new Mock<IDataBroker>();

            var services = new ServiceCollection();
            services.AddSingleton<DataBrokerService>();
            services.AddSingleton(l => _loggerMock.Object);
            services.AddSingleton(c => _configMock.Object);
            services.AddSingleton(d => _mqttClientMock.Object);
            services.AddSingleton(d => _dataBrokerMock.Object);

            _serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public async Task ExecuteAsync_RepeatingError_StopService()
        {
            //Arrange
            var config = new DataBrokerConfiguration
            {
                AlertConfiguration = new AlertConfiguration() { Enabled = false },
                CloudPublishConfiguration = new CloudPublishConfiguration() { Enabled = true, UpdateIntervalMinutes = 1, HistoryUpdateIntervalMinutes = 1 },
                DataBrokerEnabled = true,
                MqttServerAddress = "1.1.1.1"
            };

            _mqttClientMock.Setup(m => m.IsConnected).Returns(true);
            _configMock.Setup(c => c.Value).Returns(config);
            _dataBrokerMock.Setup(d => d.PublishCurrentData()).Throws(new Exception());

            //Act      
            var btGwService = _serviceProvider.GetService<DataBrokerService>();
            var cts = new CancellationTokenSource();
#pragma warning disable CS4014
            // Run as fire & forget            
            Task.Run(() => btGwService.StartAsync(cts.Token));
#pragma warning restore

            await Task.Delay(500);
            cts.Cancel();

            _dataBrokerMock.Verify(d => d.PublishCurrentData(), Times.Exactly(DataBrokerService.MaxRepeatedFailCount),
                "The service did not stop after specified amount of repeated failed operations");
        }
    }
}
