import SensorDeviceTypeModel from '../models/SensorDeviceTypeModel'
import SensorModel from '../models/SensorModel'
import SensorDeviceModel from '../models/SensorDeviceModel'
import CalculatedMeasurementModel from '../models/CalculatedMeasurementModel';
import LocationModel from '../models/LocationModel';
import MeasurementModel from '../models/MeasurementModel';
import HistoryItemModel from '../models/HistoryItemModel';

class Parser{    
    static parseLocations(data){
        if (data === undefined){
            return undefined;
        }

        const locationData = data["LocationData"];
        if (locationData === undefined){
            return undefined;
        }

        let locations = new Map();
        const sensorDevices = this.parseSensorDevices(data["SensorDevice"]);
        const sensorDeviceTypes = this.parseSensorDeviceTypes(data["SensorDeviceType"]);
        const historyData = this.parseHistoryData(data["HistoryData"]);

        for (let key in locationData){
            let location = new LocationModel(locationData[key].Id, locationData[key].Name,
                locationData[key].Timestamp, locationData[key].UpdateInterval);
            
            if (locationData[key].Measurements !== undefined){
                locationData[key].Measurements.forEach(measData => {
                    var measurementId = locationData[key].Id + "$" + measData.SensorDeviceId + "$" + measData.SensorId;
                    const measurement = new MeasurementModel(measurementId, measData.SensorDeviceId,
                        measData.SensorId, measData.Timestamp, measData.Value);
                    
                    var sensorDevice = sensorDevices.get(measData.SensorDeviceId);
                    var sensorDeviceType = sensorDeviceTypes.get(sensorDevice.sensorDeviceTypeId);
                    var sensorType = sensorDeviceType.sensors.get(measData.SensorId);
                    var historyItem = historyData.get(measData.SensorDeviceId + "$" + measData.SensorId);
                    
                    if (historyItem !== undefined)
                    {
                        measurement.historyValues = historyItem.values;
                    }

                    if (sensorType !== undefined){
                        measurement.class = sensorType.class;
                        measurement.unit = sensorType.unit;
                        measurement.name = sensorType.name;
                        
                        location.measurements.set(measurement.id, measurement);
                    }
                    else if (sensorDevice.calculatedMeasurements.has(measData.SensorId)){
                        var calcMeas = sensorDevice.calculatedMeasurements.get(measData.SensorId);
                        measurement.class = calcMeas.class;
                        measurement.unit = calcMeas.unit;
                        measurement.name = calcMeas.name;
                        
                        location.measurements.set(measurement.id, measurement);
                    }                                        

                    let storedVisibilityValue = localStorage.getItem(measurementId);
                    if (storedVisibilityValue === null){
                        if (measurement.class === 'temperature' || measurement.class === 'humidity')
                            {
                                storedVisibilityValue = 'true';
                            }                        
                    }
                    measurement.isVisible = (storedVisibilityValue === 'true') ? true : false;
                });
            }
            locations.set(location.id, location);
        }

        //console.log(locations);
        return locations;
    }
    static parseSensorDeviceTypes(sensorDeviceTypeData){
        if (sensorDeviceTypeData === undefined){
            return undefined;
        }

        let sensorDeviceTypes = new Map();
        
        for (let key in sensorDeviceTypeData){
            let type = new SensorDeviceTypeModel(sensorDeviceTypeData[key].Id,
                sensorDeviceTypeData[key].Name);                                
            
            sensorDeviceTypeData[key].Sensors.forEach(sensor => {
                let sensorModel = new SensorModel();
                sensorModel.id = sensor.Id;
                sensorModel.name = sensor.Name;                    
                sensorModel.class = sensor.Class;                    
                sensorModel.unit = sensor.Unit;
                type.sensors.set(sensorModel.id, sensorModel);
            });
            sensorDeviceTypes.set(type.id, type);
        }

        //console.log(sensorDeviceTypes);
        return sensorDeviceTypes;
    }

    static parseSensorDevices(sensorDeviceData){
        if (sensorDeviceData === undefined){
            return undefined;
        }

        let sensorDevices = new Map();
        
        for (let key in sensorDeviceData){
            let sensorDevice = new SensorDeviceModel(sensorDeviceData[key].Id,
                sensorDeviceData[key].Location, sensorDeviceData[key].SensorDeviceTypeId);                                
            
            if (sensorDeviceData[key].CalculatedMeasurements !== undefined) {
                sensorDeviceData[key].CalculatedMeasurements.forEach(meas => {
                    let measModel = new CalculatedMeasurementModel(meas.Id, meas.Class,
                        meas.Name, meas.Operator, meas.SensorId, meas.Type, meas.Unit, 
                        meas.Value);                    
                    sensorDevice.calculatedMeasurements.set(measModel.id, measModel);
                });
            }
            
            sensorDevices.set(sensorDevice.id, sensorDevice);
        }

        //console.log(sensorDevices);
        return sensorDevices;
    }

    static parseHistoryData(rawHistoryData){                
        if (rawHistoryData === undefined){
            return undefined;
        }

        let historyDataItems = new Map();
        var rawDataList = Object.entries(rawHistoryData)[0][1];
        if (rawDataList.DataList !== undefined){
            rawDataList.DataList.forEach(rawDataItem => {
                var itemId = rawDataItem.SensorDeviceId + "$" + rawDataItem.SensorId;
                const historyItem = new HistoryItemModel(itemId, rawDataItem.SensorDeviceId, rawDataItem.SensorId);
                
                rawDataItem.Values.forEach(rawValueItem => {
                    //const valueItem = new HistoryItemValueModel(new Date(rawValueItem.Timestamp), rawValueItem.Value);
                    //historyItem.values.push({date: new Date(rawValueItem.Timestamp).getDay(), value: parseFloat(rawValueItem.Value)});
                    historyItem.values.push([new Date(rawValueItem.Timestamp), parseFloat(rawValueItem.Value)]);
                });
                historyDataItems.set(itemId, historyItem);                
            });
        }
        
        return historyDataItems;
    }
}

export default Parser;