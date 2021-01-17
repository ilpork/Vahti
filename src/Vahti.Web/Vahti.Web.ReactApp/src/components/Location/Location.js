import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faChevronRight } from '@fortawesome/free-solid-svg-icons';
import Measurement from '../Measurement/Measurement';
import './Location.css';

const Location = (props) => {     
    return (
        <div className="Location" onClick={props.clicked}>   
            <div>
                <div className="Location__header">
                    <h4 className="Location__header__title">{props.name}</h4>
                    <FontAwesomeIcon className="Location__header__caret" icon={faChevronRight} size="1x"/>    
                </div>
                <div className="Location__measurementList">
                {Array.from(props.measurements).filter(([index,m]) => {
                    return m.isVisible;
                }).map(([index, m]) => {                    
                    return (
                        <Measurement key={m.id} name={m.name} value={m.value} unit={m.unit} class={m.class}/>)
                    
                })}            
                </div>
            </div>            
        </div>
    );
}

export default Location;