class CalculatedMeasurementModel {
    constructor(id, className, name, operator, sensorId, type, unit, value){
        this._id = id;
        this._class = className;
        this._name = name;
        this._operator = operator;
        this._sensorId = sensorId;
        this._type = type;
        this._unit = unit;
        this._value = value;
    }

    get id(){
        return this._id;
    }

    get class(){
        return this._class;
    }

    get name(){
        return this._name;
    }

    get operator(){
        return this._operator;
    }

    get sensorId(){
        return this._sensorId;
    }

    get type(){
        return this._type;
    }

    get unit() {
        return this._unit;
    }

    get value() {
        return this._value;
    }
}

export default CalculatedMeasurementModel;