class MeasurementModel {
    constructor(id, sensorDeviceId, sensorId, timestamp, value){
        this.id = id;
        this.sensorDeviceId = sensorDeviceId;
        this.sensorId = sensorId;
        this.timestamp = timestamp;
        this.value = value;
        this.class = undefined;
        this.unit = undefined;
        this.name = undefined;
        this.isVisible = true;
        this.historyValues = [];
    }    
}

export default MeasurementModel;