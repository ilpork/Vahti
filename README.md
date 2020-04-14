# Vahti
[![Build Status](https://dev.azure.com/ilpork/github/_apis/build/status/ilpork.Vahti?branchName=master)](https://dev.azure.com/ilpork/github/_build/latest?definitionId=3&branchName=master)

Vahti is a lightweight .NET and MQTT based home monitoring system to read measurement data from different kind of sources. 

It consists of server part (running on Raspberry Pi, for example) to gather data and and mobile app to show it. Alerts (push/email) are also supported. System can also be used without mobile app just to get alerts by email from server, or to publish data for any generic MQTT client application.

## General concepts
- Lightweight. Server runs well on Raspberry Pi (2/3/4 with ARMv7/8)
- No need to expose home network to Internet (data is stored in cloud database)
- No paid services needed
- Possible to add new sensors without code changes
- Can be used without mobile app just as an alert system
- Different services can also be run on different machines 

## Features
### Mobile application (Vahti.Mobile)
![Locations](doc/images/locations.png) 
![Locations](doc/images/history.png)
![Locations](doc/images/details.png)
- Implemented with Xamarin Forms (focus has been on Android because iOS compiled with Apple Developer license works only a week at time, so it's not very appealing)
- Show latest measurement data and history graphs for measurement data
- Choose which measurements to show in UI
- Get push notifications (alerts) from server 
- Localizable (currently supports Finnish and English)
- No background polling done
### Server (Vahti.Server)
Server can be configured to run all or any of the services below. All services can run on same machine, or they can be distributed to different machines.
#### DataBroker (Vahti.DataBroker)
- Implemented with .NET Core 3.0
- Uses MQTT to gather measurement data from any MQTT client publishing data in specific format
- Sends measurements and history data periodically to cloud database (currently Google Firebase)
- Send alerts as push notifications to the mobile application, or as email to any device
- In addition to handling pure measurement values, supports also publishing of processed values
- Configurable using configuration file
#### Bluetooth gateway (Vahti.BluetoothGw)
- Implemented with .NET Core 3.0
- Scan Bluetooth LE devices and publish their data
- Reads manufacturer raw data from any device
- Currently supports parsing data of [RuuviTag](https://www.ruuvi.com), but support for other devices can be added
- Configurable using configuration file
- Uses my [BleReader.Net](https://github.com/ilpork/BleReader.Net) to read BLE data
#### MQTT server
- Wraps the [MQTTnet](https://github.com/chkr1011/MQTTnet) server to provide MQTT server functionality

## System overview
![Overview diagram](doc/images/overview.png)

## Requirements
For sending alerts by email or to publish for a generic MQTT client:
1) A server (Linux/Windows/Mac) where to run the server part (Vahti.DataBroker, Vahti.BluetoothGw and Vahti.Mqtt). Raspberry Pi 2/3/4 is fine for that purpose
2) Have some measurement sources to publish measurement data using MQTT

For full setup with mobile app and push notifications requires in addition:
1. Visual Studio 2019 (Community edition is fine) to build the mobile app
2. Android device (app should work on iOS too, but requires Apple developer license, so iOS version has been on side track)
3. Google Firebase realtime database needed for data storage to use the mobile app (free)
4. Google Firebase project needed to get Push notifications on Android (free) 
5. Microsoft Azure account and notification hub needed for push notifications (free)


## Usage
Depending on configuration and functionality used, the system requires different kind of setup. See details with tutorials  in [Getting started](doc/GettingStarted.md)

## Background
Originally I did a very simple version of the system for my own purposes to supervise my robotic lawnmower by using a RuuviTag sensor, and show that information along with other available sensor information anywhere I am, without exposing my home network to Internet. Then I thought that maybe I could make it more generic, so someone could use it too.

I try to document the project so, that also those with less development experience could use it.

Vahti is a Finnish word. It means guard, watch or sentry in English. 

## Contribute
You can contribute by submitting bugs, feature requests and pull requests. 

## License

Copyright (c) ilpork. All rights reserved.

Licensed under the MIT license.