using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vahti.DataBroker.AlertHandler;
using Vahti.DataBroker.Configuration;
using Vahti.DataBroker.DataProvider;
using Vahti.Shared;
using Vahti.Shared.Data;
using Vahti.Shared.DataProvider;

namespace Vahti.DataBroker.Test.DataBroker
{
    /// <Summary>
    /// Unit tests for <see cref="Vahti.DataBroker.DataBroker.DataBroker>"/>
    /// </Summary>
    [TestClass]
    public class DataBrokerTests
    {
        private Mock<ILogger<Vahti.DataBroker.DataBroker.DataBroker>> _loggerMock;
        private Mock<IOptions<DataBrokerConfiguration>> _configMock;
        private Mock<IDataProvider> _dataProvider;
        private Mock<IHistoryDataProvider> _historyDataProvider;
        private Mock<IAlertHandler> _alertHandler;

        [TestInitialize]
        public void InitializeTest()
        {
            _loggerMock = new Mock<ILogger<Vahti.DataBroker.DataBroker.DataBroker>>();
            _configMock = new Mock<IOptions<DataBrokerConfiguration>>();
            _dataProvider = new Mock<IDataProvider>();
            _historyDataProvider = new Mock<IHistoryDataProvider>();
            _alertHandler = new Mock<IAlertHandler>();
        }

        [TestMethod]
        public async Task PublishHistoryData_HistoryDataAdded_True()
        {
            bool sensorHistoryDataStored = false;
            bool calcMeasurementHistoryDataStored = false;

            const string sensorId = "mySensorId";
            const string calcSensorId = "myCalcId";            
            const string sensorDeviceId = "mySensorDeviceId";            
            _configMock.Setup(config => config.Value).Returns(
                new DataBrokerConfiguration()
                {
                    CloudPublishConfiguration = new CloudPublishConfiguration()
                    {
                        Enabled = true,
                        HistoryLengthDays = 1                        
                    }
                });
            _dataProvider.Setup(dp => dp.StoreNewItemAsync<HistoryData>(It.IsAny<HistoryData>()))
                .Callback<HistoryData>((historyData) =>
                {
                    calcMeasurementHistoryDataStored = historyData.DataList.Count(item => item.SensorDeviceId.Equals(sensorDeviceId) &&
                        item.SensorId.Equals(calcSensorId)) == 1;                    
                    sensorHistoryDataStored = historyData.DataList.Count(item => item.SensorDeviceId.Equals(sensorDeviceId) &&
                        item.SensorId.Equals(sensorId)) == 1;
                });

            _historyDataProvider.Setup(hdp => hdp.GetHistory(sensorDeviceId, It.IsAny<string>(), 
                _configMock.Object.Value.CloudPublishConfiguration.HistoryLengthDays))
                .ReturnsAsync(new List<MeasurementData>());
            var dataBroker = new Vahti.DataBroker.DataBroker.DataBroker(_loggerMock.Object,
                _configMock.Object, _dataProvider.Object, _historyDataProvider.Object, 
                _alertHandler.Object);

            var sensorDeviceType = @"{ ""id"": ""deviceTypeId"", ""name"": ""deviceName"", ""manufacturer"": " +
                @"""deviceManufacturer"", ""sensors"": [{ ""id"": """ + sensorId + @""", ""name"": " +
                @"""mySensorName"", ""class"": ""mySensorClass"", ""unit"": ""myUnit""}]}";
            var sensorDevice = @"{""id"": """ + sensorDeviceId + @""", ""address"": ""myAddress"", " +
                @"""name"": ""mySensorName"", ""sensorDeviceTypeId"": ""deviceTypeId"", " +
                @"""location"": ""myLocation"", ""calculatedMeasurements"": [{ ""id"": " +
                @"""" + calcSensorId + @""", ""name"": ""myCalcName"", ""class"": ""myCalcClass"", " +
                @"""unit"": ""myCalcUnit"", ""type"": 1, ""sensorId"": ""mySensorId"", " +
                @"""operator"": 0, ""value"": ""1"" }]}";
            dataBroker.MqttMessageReceived(Constant.TopicSensorDeviceType, sensorDeviceType);
            dataBroker.MqttMessageReceived(Constant.TopicSensorDevice, sensorDevice);

            await dataBroker.PublishHistoryData();
                        
            Assert.IsTrue(calcMeasurementHistoryDataStored, "History data for calculated measurement was not stored");
            Assert.IsTrue(sensorHistoryDataStored, "History data for sensor was not stored");
        }
    }
}
