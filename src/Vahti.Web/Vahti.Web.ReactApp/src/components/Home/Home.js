import React from 'react';
import { useHistory } from 'react-router-dom';
import LocationList from '../LocationList/LocationList'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCog } from '@fortawesome/free-solid-svg-icons';
import './Home.css';
import SettingStorage from '../../classes/SettingStorage';

const Home = (props) => {   
    const history = useHistory();

    const onClicked = (locationId) => {
        props.clicked(locationId);
        history.push("/graphs");
    }

    let body = null;
    if (props.locations === null) {
        body = <p>Default data source has not been defined. Enter data source on settings page.</p>        
    }
    else if (props.locations === undefined) {
        if (process.env.NODE_ENV === 'production') {
            const dataSource = SettingStorage.getDataSource();
            body = <p>Could not read valid data from {dataSource}</p>        
        }
        else {
            body = <p>Could not parse the mock data</p>        
        }
    }
    else {
        body = <LocationList locations={props.locations} clicked={onClicked}/>
    }

    return (
        <div className="Page">      
            <header className="Home__header">
                <h4 className="Home__header__title">Vahti</h4>
                <a className="Home__header__settingsIcon" href="/settings">
                    <FontAwesomeIcon  icon={faCog}/>
                </a>            
            </header>
            <div className="Body">  
                {body}
            </div>      
        </div>
    );
}

export default Home;