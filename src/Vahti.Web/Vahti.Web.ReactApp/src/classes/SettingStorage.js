class SettingStorage {
    static toggleVisibility(locations, measurementIndex){
        const [locationId,,] = measurementIndex.split('$');        
        const measurement = locations.get(locationId).measurements.get(measurementIndex);
        measurement.isVisible = !measurement.isVisible;
    
        localStorage.setItem(measurementIndex, measurement.isVisible);
    }

    static setLastSelectedLocation(locationId){
        localStorage.setItem("lastSelectedLocation", locationId);
    }

    static getLastSelectedLocation() {
        return localStorage.getItem("lastSelectedLocation");
    }

    static setDataSource(dataSource){
        localStorage.setItem("dataSource", dataSource);
    }

    static getDataSource() {
        let dataSource = localStorage.getItem("dataSource");
        if (dataSource === null){
            dataSource = process.env.REACT_APP_DATA_SOURCE;
        }
        return dataSource;
    }
}

export default SettingStorage;