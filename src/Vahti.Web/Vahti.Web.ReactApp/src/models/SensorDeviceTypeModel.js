class SensorDeviceTypeModel {    
    constructor(id, name){
        this._id = id;
        this._name = name;
        this.sensors = new Map();
    }
    get name(){
        return this._name;
    }
    set name(newName){
        this._name = newName;
    }

    get id(){
        return this._id;
    }
    set id(newId){
        this._id = newId;
    }

    get sensors(){
        return this._sensors;
    }
    set sensors(newSensors){
        this._sensors = newSensors;
    }
}

export default SensorDeviceTypeModel;