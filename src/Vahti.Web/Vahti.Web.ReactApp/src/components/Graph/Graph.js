import React from 'react';
import { Resizable, Charts, ChartContainer, ChartRow, YAxis, AreaChart } from "react-timeseries-charts";
import { TimeSeries } from "pondjs";
import { styler } from "react-timeseries-charts";
import './Graph.css';

const Graph = (props) => {     
    if (props.historyValues.length === 0){
        return <div/>
    }

    const points = props.historyValues;
    const series = new TimeSeries({
        name: "history",
        columns: ["time", "value"],
        points
    });
    
    const chartStyle = styler([{ key: "value", color: "#aaaaaa" }]);
    const yAxisStyle = {
        label: {
            stroke: "none",
            fill: "#ddd", 
            fontWeight: 200,
            fontSize: 14            
        },
        values: {
            stroke: "none",
            fill: "#ddd",
            fontWeight: 100,
            fontSize: 11
        },
        ticks: {
            fill: "none",
            stroke: "#444",
            opacity: 1
        },
        axis: {
            fill: "none",
            stroke: "#ddd",
            opacity: 1            
        }
    };

    const timeAxisStyle = {
        label: {
            stroke: "none",
            fill: "#ddd", 
            fontWeight: 200,
            fontSize: 14            
        },
        values: {
            stroke: "none",
            fill: "#ddd",
            fontWeight: 100,
            fontSize: 11            
        },
        ticks: {
            fill: "none",
            stroke: "#888",
            opacity: 1,
            showGrid: true
        },
        axis: {
            fill: "none",
            stroke: "#ddd",
            opacity: 1            
        }
    };
    
    const getMin = (className, seriesMin) =>
    {
        switch (className)
        {
            case "temperature":
                return seriesMin - 5;
            case "humidity":
                return 0;
            case "pressure":
                return 940;
            default:
                return seriesMin;
        }
    }

    const getMax = (className, seriesMax) =>
    {
        switch (className)
        {
            case "temperature":
                return seriesMax + 5;
            case "humidity":
                return 100;
            case "pressure":
                return 1066;
            default:
                return seriesMax;
        }
    }

    const getFormat = (className) => {
        switch (className)
        {
            case "temperature":
                return ".1f";
            case "humidity":
            case "pressure":
                return ".0f";
            case "accelerationX":
            case "accelerationY":
            case "accelerationZ":
                return ".3f";
            default:
                return ".1f";
        }
    }
  
    return (   
        <Resizable className="Graph">
            <ChartContainer 
                title={props.title} 
                titleStyle={{ fill: "#ddd", fontWeight: 500 }} 
                timeAxisStyle={timeAxisStyle}                   
                timeAxisAngledLabels
                timeAxisHeight={50}
                timeRange={series.range()}>
                <ChartRow height="200">
                    <YAxis 
                        id="axis1" 
                        type="linear" 
                        showGrid 
                        tickCount={11} 
                        style={yAxisStyle} 
                        min={getMin(props.className, series.min())} 
                        max={getMax(props.className, series.max())} 
                        format={getFormat(props.className)}
                        width="40"                         
                        />
                    <Charts>
                        <AreaChart axis="axis1" series={series} style={chartStyle}/>            
                    </Charts>        
                    <YAxis 
                        id="axis1" 
                        type="linear" 
                        showGrid 
                        tickCount={11} 
                        style={yAxisStyle} 
                        min={getMin(props.className, series.min())} 
                        max={getMax(props.className, series.max())} 
                        format={getFormat(props.className)}
                        width="40"                         
                        />
                </ChartRow>
            </ChartContainer>
        </Resizable>
    );
}

export default Graph;