import React from 'react';
import Dots from 'react-activity/lib/Dots';
import 'react-activity/lib/Dots/Dots.css';
import Location from '../Location/Location';
import './LocationList.css';

const LocationList = (props) =>
{    
    if (props.locations === undefined || props.locations.size === 0){        
        return (
            <div>
                <p className="LocationList__updating">Updating</p>
                <Dots/>
            </div>
            );
    }
    
    return (<div className="LocationList__list">
        {Array.from(props.locations).map(([index, location]) => {            
            return (
            <Location key={location.id} name={location.name} measurements={location.measurements} clicked={() => props.clicked(index)}/>)
        })}
        </div>);
}

export default LocationList;