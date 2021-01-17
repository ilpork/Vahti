import Parser from '../classes/Parser';

class DataFetcher {    
    static async fetchLocationData(dataSource) {
        let data;
        if (process.env.NODE_ENV === 'production') {
            if (dataSource === undefined)
            {
                return null;
            }

            const api_call = await fetch(dataSource);
            data = await api_call.json();               
        }
        else {   
            data = await import('../mocks/MockData.json');                                 
        }      

        return Parser.parseLocations(data);             
    }    
}

export default DataFetcher;