# Getting started
Starting to use Vahti requires setting up different kind of things depending on what you want to achieve. 

[Full setup tutorial](FullTutorial.md) describes how to set up system where full functionality of Vahti is used: read data using Vahti.BluetoothGw, relay data to cloud database using Vahti.DataBroker and show it in mobile app.

[Alert tutorial](AlertTutorial.md) describes how to get just alerts without having mobile app

If you want to have other devices to publish their data using MQTT via Vahti, check [Add other data sources](AddOtherDataSources.md).

Mobile app only shows the information provided by Vahti.Server. The current approach is quite limiting but clear. Vahti.Server publishes measurement data so, that mobile app knows how the data should be shown. In practice it happens by declaring which kind of data do they send.

`Sensor.Class` is an essential part of it. Each sensor, like temperature sensor, must define its class. For temperature sensor it's `temperature`. That way mobile app knows that it's temperature data, and should be shown in UI in certain way. 
```
{
    "id": "temperature",
    "name": "Temperature",
    "class": "temperature",
    "unit": "°C"
}
```

Each `SensorDevice`, respectively, has `Location` defined. Location is used to group different measurements under same header in mobile app.
```
{
    "id": "ruuviGarage",
    "address": "12:34:56:78:90:AB",
    "name": "RuuviTag in garage",
    "sensorDeviceTypeId": "RuuviTag",
    "location": "Garage"
},
```

