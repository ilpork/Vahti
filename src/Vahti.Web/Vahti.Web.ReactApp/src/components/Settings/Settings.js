import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faLongArrowAltLeft } from '@fortawesome/free-solid-svg-icons';
import SettingsList from '../SettingsList/SettingsList';
import { useHistory } from 'react-router-dom';
import './Settings.css';

const Settings = (props) => {
    const history = useHistory();

    const onDataSourceChanged = () => {                
        history.push("/");
        props.dataSourceChanged();
    }
    return (
        <div>      
            <header className="ChildPage__header">                    
              <div className="ChildPages__header__title">
                <a href="/">
                  <FontAwesomeIcon icon={faLongArrowAltLeft} size="2x"/>
                </a>
                <h2 className="ChildPage__header_title_text">Settings</h2>
              </div>
            </header>
            <div className="Settings__body">                        
              <SettingsList 
                locations={props.locations} 
                visibilitySettingChanged={props.visibilitySettingChanged}
                dataSourceChanged={onDataSourceChanged}/>
            </div>      
          </div>
    );
}

export default Settings;