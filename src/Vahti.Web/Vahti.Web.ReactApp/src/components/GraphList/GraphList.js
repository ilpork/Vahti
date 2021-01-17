import React from 'react';
import Graph from '../Graph/Graph';
import './GraphList.css';

const GraphList = (props) => {
    if (props.location === undefined ||
        props.location.measurements === undefined){
        return <div/>;
    }
    
    return (
        <div className="Graph__list">
            {Array.from(props.location.measurements).filter(([index,m]) => {
                return m.isVisible;
            }).map(([index, m]) => {
                return (
                    <Graph 
                        key={m.id} 
                        historyValues={m.historyValues} 
                        title={m.name + " (" + m.unit + ")"} 
                        className={m.class}/>)                
            })}       
        </div>
    );
}

export default GraphList;