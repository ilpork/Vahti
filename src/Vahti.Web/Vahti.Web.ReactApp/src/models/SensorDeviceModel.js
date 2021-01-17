class SensorDeviceModel {
    constructor(id, location, sensorDeviceTypeId){
        this._id = id;
        this._location = location;
        this._sensorDeviceTypeId = sensorDeviceTypeId;
        this._calculatedMeasurements = new Map();
    }

    get id(){
        return this._id;
    }
    set id(newId){
        this._id = newId;
    }

    get location(){
        return this._location;
    }
    set location(newLocation){
        this._location = newLocation;
    }

    get sensorDeviceTypeId() {
        return this._sensorDeviceTypeId;
    }
    set sensorDeviceTypeId(newSensorDeviceTypeId){
        this._sensorDeviceTypeId = newSensorDeviceTypeId;
    }

    get calculatedMeasurements(){
        return this._calculatedMeasurements;
    }
}

export default SensorDeviceModel;