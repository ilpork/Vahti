import { Barometer, DirectionDown, DirectionRight, DirectionUpLeft, Humidity, Thermometer } from "@intern0t/react-weather-icons";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBatteryHalf } from '@fortawesome/free-solid-svg-icons';
import { faArrowsAlt } from '@fortawesome/free-solid-svg-icons';
import { faChargingStation } from '@fortawesome/free-solid-svg-icons';

import './Measurement.css';

const Measurement = (props) => {
    function getIcon(className){
        const size = "40";
        const color = "#fff"
        switch (className){
            case "temperature":
                return <Thermometer className="Measurement__weatherIcon" color={color}  size={size}/>;
            case "humidity":
                return <Humidity className="Measurement__weatherIcon"  color={color} size={size}/>;
            case "pressure":
                return <Barometer className="Measurement__weatherIcon"  color={color} size={size}/>;
            case "accelerationX":
                return <DirectionRight className="Measurement__weatherIcon"  color={color} size={size}/>;
            case "accelerationY":
                return <DirectionDown className="Measurement__weatherIcon"  color={color} size={size}/>;
            case "accelerationZ":
                return <DirectionUpLeft className="Measurement__weatherIcon"  color={color} size={size}/>;
            case "batteryvoltage":
                return <FontAwesomeIcon className="Measurement__fontAwesomeIcon" icon={faBatteryHalf} size="sm"/>;
            case "movementCounter":
                return <FontAwesomeIcon className="Measurement__fontAwesomeIcon" icon={faArrowsAlt}/>;
            case "lastCharged":
                return <FontAwesomeIcon className="Measurement__fontAwesomeIcon" icon={faChargingStation}/>;
            default:
                return <div/>;
        }
    }

    function formatValue(className, value){
        switch (className){
            case "temperature":
                return parseFloat(value).toFixed(1);
            case "humidity":
                return parseFloat(value).toFixed(0);
            case "pressure":
            case "movementCounter":
            case "lastCharged":
                return parseFloat(value).toFixed(0);            
            default:
                return parseFloat(value).toFixed(3);
        }
    }
    
    return (
        <div className="Measurement">
            <div className="Measurement__icon">{getIcon(props.class)}</div>
            <div className="Measurement__value">{formatValue(props.class, props.value)} {props.unit}</div>
        </div>
    );
}

export default Measurement;