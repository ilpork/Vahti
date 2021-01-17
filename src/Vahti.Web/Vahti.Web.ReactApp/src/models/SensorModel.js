class SensorModel {
    get id(){
        return this._id;
    }
    set id(newId){
        this._id = newId;
    } 

    get name(){
        return this._name;
    }
    set name(newName){
        this._name = newName;
    }

    get class(){
        return this._class;
    }
    set class(newClass){
        this._class = newClass;
    }

    get unit(){
        return this._unit;
    }
    set unit(newUnit){
        this._unit = newUnit;
    }
}

export default SensorModel;