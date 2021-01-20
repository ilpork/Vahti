class LocationModel {
    constructor(id, name, timestamp, updateInterval){
        this.id = id;
        this._name = name;
        this.timestamp = timestamp;
        this.updateInterval = updateInterval;
        this.measurements = new Map();
    }    

    get name(){
        return this._name;
    }

    set name(newName){
        this._name = newName;
    }
}

export default LocationModel;