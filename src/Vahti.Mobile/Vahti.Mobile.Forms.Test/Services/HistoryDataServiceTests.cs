using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vahti.Mobile.Forms.Services;
using Vahti.Shared.Data;
using Vahti.Shared.DataProvider;
using Vahti.Shared.TypeData;

namespace Vahti.Mobile.Forms.Test.Services
{
    /// <summary>
    ///  Unit tests for <see cref="HistoryDataService"/>
    /// </summary>
    [TestClass]
    public class HistoryDataServiceTests
    {        
        [TestMethod]
        public async Task GetAllDataAsync_CustomMeasurementHandledCorrectly()
        {
            var customUnit = "myCustomUnit";
            var customSensorId = "myCustomSensorId";

            var sensorDeviceTypeList = new List<SensorDeviceType>()
            {
                new SensorDeviceType()
                {
                    Id = "mySensorDeviceTypeId",
                    Name = "mySensorDeviceTypeName",
                    Sensors = new List<Sensor>()
                    {
                        new Sensor()
                        {
                            Id = "mySensorId"
                        }
                    }
                }
            };

            var sensorDeviceList = new List<SensorDevice>()
            {
                new SensorDevice()
                {
                    Id = "mySensorDeviceId",
                    SensorDeviceTypeId = "mySensorDeviceTypeId",
                    CalculatedMeasurements = new List<CustomMeasurementRule>()
                    {
                        new CustomMeasurementRule()
                        {
                            Id = customSensorId,
                            Unit = customUnit
                        }
                    }
                }
            };

            var historyDataList = new List<HistoryData>()
            {
                new HistoryData()
                {
                    DataList = new List<MeasurementHistoryData>()
                    {
                        new MeasurementHistoryData()
                        {
                            SensorDeviceId = "mySensorDeviceId",
                            SensorId = customSensorId,
                            Values = new List<HistoryValueData>()
                            {
                                new HistoryValueData()
                                {
                                    Timestamp = DateTime.Now,
                                    Value = "1"
                                }
                            }
                        }
                    }
                }
            };

            var dataProvider = new Mock<IDataProvider>();
            dataProvider.Setup(dp => dp.LoadAllItemsAsync<SensorDeviceType>())
                .ReturnsAsync(sensorDeviceTypeList);
            dataProvider.Setup(dp => dp.LoadAllItemsAsync<SensorDevice>())
                .ReturnsAsync(sensorDeviceList);
            dataProvider.Setup(dp => dp.LoadAllItemsAsync<HistoryData>())
                .ReturnsAsync(historyDataList);

            var historyDataService = new HistoryDataService(dataProvider.Object);

            var historyItems = await historyDataService.GetAllDataAsync(true);

            Assert.AreEqual(1, historyItems.Count, "There should be one history item");
            Assert.AreEqual(customSensorId, historyItems.First().SensorId, "Incorrect SensorId");
            Assert.AreEqual(customUnit, historyItems.First().Unit, "Incorrect Unit");
        }
    }
}
