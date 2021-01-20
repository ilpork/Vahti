import React from 'react';
import Setting from '../Setting/Setting';
import './SettingGroup.css';

const SettingGroup = (props) => {     
    return (
        <div className="SettingGroup">            
            <h4 className="SettingGroup__header">{props.name}</h4>
            <div className="SettingGroup__settings">
            {Array.from(props.measurements).map(([index, m]) => {                
                    return (
                        <Setting 
                            key={m.id}
                            name={m.name} 
                            id={m.id} 
                            isChecked={m.isVisible} 
                            visibilitySettingChanged={event => props.visibilitySettingChanged(event, m.id)} />)
                
            })}            
            </div>
        </div>
    );
}

export default SettingGroup;