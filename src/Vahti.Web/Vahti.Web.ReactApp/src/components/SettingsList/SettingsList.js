import React, { Component } from 'react';
import SettingStorage from '../../classes/SettingStorage';
import SettingGroup from '../SettingGroup/SettingGroup';
import './SettingsList.css';

class SettingsList extends Component {            
    constructor(){
        super();
        this.state = { showVisibilitySettings: true };
    }

    dataSourceChanged = (event) => {
        this.dataSource = event.target.value;
        this.updateVisibilitySettings();
    }

    dataSourceChangeCommitted = () => {
        SettingStorage.setDataSource(this.dataSource);
        this.originalDataSource = this.dataSource;
        this.updateVisibilitySettings();
        this.props.dataSourceChanged();
    }

    dataSourceChangeCanceled = () => {        
        this.dataSource = this.originalDataSource;
        this.updateVisibilitySettings();
    }

    componentDidMount(){
        this.dataSource = SettingStorage.getDataSource();                
        this.originalDataSource = this.dataSource;
    }
    
    updateVisibilitySettings = () => {
        this.setState({showVisibilitySettings: this.originalDataSource === this.dataSource});
    }

    render() {
        if (this.props.locations != null && (this.props.locations === undefined || this.props.locations.size === 0)){
            return (
                <div className="SettingsList">
                <p className="SettingsList__updating">Updating...</p>
                </div>);
        }

        let visibilitySettingList = null;
        let dataSourceButtons = null;
        let isValidDataSource = this.props.locations !== null && this.props.locations !== undefined;
        let visibilityListHeader = isValidDataSource ? 
            <p className="SettingsList__description">Choose items to show</p> :
            <p className="SettingsList__description">Enter valid data source to see and configure item visibility</p>

        if (this.state.showVisibilitySettings)
        {   
            visibilitySettingList = 
            <div>
                {visibilityListHeader}
                {isValidDataSource && Array.from(this.props.locations).map(([index, location]) => {            
                    return (
                    <SettingGroup 
                        key={location.id}
                        name={location.name} 
                        measurements={location.measurements} 
                        visibilitySettingChanged={this.props.visibilitySettingChanged}/>)
                })}
            </div>            
        }
        else
        {
            dataSourceButtons =
            <div className="SettingsList__dataSource__buttons">
                <button 
                    className="SettingsList__dataSource__button SettingsList__dataSource__button--commit"
                    onClick={this.dataSourceChangeCommitted}>
                    Apply
                </button>
                <button 
                    className="SettingsList__dataSource__button"
                    onClick={this.dataSourceChangeCanceled}>
                    Cancel
                </button>
            </div>
        }
        
        return (<div className="SettingsList">
        <div className="SettingsList__dataSource">
                <p className="SettingsList__dataSource__label">Data source:</p>
                <input 
                    className="SettingsList__dataSource__value" 
                    type="text" 
                    onChange={(event) => this.dataSourceChanged(event)} 
                    value={this.dataSource}/>
                {dataSourceButtons}
              </div>     
        {visibilitySettingList}        
        </div>);
    }    
}

export default SettingsList;