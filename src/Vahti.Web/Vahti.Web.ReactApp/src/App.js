import './styles/Common.css';
import React, { Component} from 'react';
import { BrowserRouter, Switch, Route } from 'react-router-dom';
import SettingStorage from './classes/SettingStorage';
import Home from './components/Home/Home';
import Settings from './components/Settings/Settings';
import DataFetcher from './classes/DataFetcher';
import Graphs from './components/Graphs/Graphs';

let hidden = null;
let visibilityChange = null;
if (typeof document.hidden !== 'undefined') { // Opera 12.10 and Firefox 18 and later support 
  hidden = 'hidden';
  visibilityChange = 'visibilitychange';
} 
else if (typeof document.msHidden !== 'undefined') {
  hidden = 'msHidden';
  visibilityChange = 'msvisibilitychange';
} 
else if (typeof document.webkitHidden !== 'undefined') {
  hidden = 'webkitHidden';
  visibilityChange = 'webkitvisibilitychange';
}

class App extends Component {
  constructor(props){
    super(props);    
    this.state = {       
      locations: new Map(),
      selectedLocation: {},      
    }
  }
  

  handleVisibilityChange = async () => {
    if (!document[hidden]) {               
      await this.updateData();
    }
  }

  async tick() {
    var currentDate = new Date();

    if (this.state.locations !== undefined || this.state.locations != null){
      for (let [, location] of this.state.locations) {
        const locationTimestamp = new Date(location.timestamp);
        if (currentDate > locationTimestamp.setMinutes(locationTimestamp.getMinutes() + location.updateInterval + 2)){
          await this.updateData();
          console.log(`Updated values at ${currentDate.toLocaleDateString()} ${currentDate.toLocaleTimeString()}`);
          break;
        }      
      };
    }    
  }

  async componentDidMount(){
    this.interval = setInterval(() => this.tick(), 60000);
    document.addEventListener(visibilityChange, this.handleVisibilityChange, false);
    await this.updateData();
    const latestSelectedLocation = SettingStorage.getLastSelectedLocation();
    
    if (latestSelectedLocation !== null){
      const location = this.state.locations.get(latestSelectedLocation);

      if (location !== undefined){
        this.setState({selectedLocation: location});    
      }      
    }
  }

  componentWillUnmount() {
    clearInterval(this.interval);
    document.removeEventListener(visibilityChange, this.handleVisibilityChange);    
  }
  
  updateData = async() => {
    const dataSource = SettingStorage.getDataSource();
    const locations = await DataFetcher.fetchLocationData(dataSource);    
  
    this.setState({
      locations: locations      
      });    
  }
  
  onDataSourceChanged = () => {
    this.updateData();
  }

  onVisibilitySettingChanged = (event, measurementIndex) => {
    SettingStorage.toggleVisibility(this.state.locations, measurementIndex);
    this.setState({ locations: this.state.locations});    
  }

  locationSelected = (locationId) => {
    const location = this.state.locations.get(locationId);
    this.setState({selectedLocation: location});        
    SettingStorage.setLastSelectedLocation(locationId);    
  }
  
  render()
  { 
    return (
      <div>
        <BrowserRouter>          
          <Switch>
            <Route exact path="/" render={() => 
              <Home 
                locations={this.state.locations} 
                clicked={this.locationSelected}/>}/>              
            <Route path="/settings" render={() => 
              <Settings 
                locations={this.state.locations} 
                visibilitySettingChanged={this.onVisibilitySettingChanged}
                dataSourceChanged={this.onDataSourceChanged} />}/>
            <Route path="/graphs" render={() => 
              <Graphs 
                location={this.state.selectedLocation}/>} />            
          </Switch>          
        </BrowserRouter>
      </div>
    );
  };
}

export default App;
