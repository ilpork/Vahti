class HistoryItemModel{
    constructor(id, sensorDeviceId, sensorId){
        this.id = id;
        this.sensorDeviceId = sensorDeviceId;
        this.sensorId = sensorId;
        this.values = [];
    }    
}

export default HistoryItemModel;