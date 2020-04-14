# Configuration
The server side applications are configured using configuration file (`config.json`), which is placed in same folder as where the application binaries are. You can use `config.template.json` as base by copying it to `config.json`. 

The can contain multiple configuration sections depending on application. There's one section per service (BluetoothGw, DataBroker, Mqtt) and also logging section can be used. Vahti.App.Full contains all services in one app, so its configuration file must contain all sections.

## MqttConfiguration
It's simple. Either the MQTT service is enabled or not. If you don't have another MQTT service running, then this should be true. MQTT service is a hub for published MQTT messages containing sensor data.
|Property|Purpose|
|--------|-------|
|mqttConfigurationEnabled|Indicates if MQTT server is enabled|

```
"mqttConfiguration": {
    "mqttServerEnabled": true
  },
```


## DataBrokerConfiguration
DataBroker is responsible for gathering the measurement data published by another MQTT clients. DataBroker is an MQTT client itself. Disable this makes sense in situations where Vahti.App.Full is used, but not all of its functionality, so for example the DataBroker is running on different machine than other part. If not alerts are configured the whole `alertConfiguration` section can be removed.

|Property|Purpose|
|--------|-------|
|dataBrokerConfigurationEnabled|Indicates if data broker service is enabled|
|mqttServerAddress|Address of the MQTT server|
|cloudPublishConfiguration|Cloud publishing configuration|

### CloudPublishConfiguration
Defines cloud database access details and how often data is published.

|Property|Purpose|
|--------|-------|
|Enabled|Indicates if cloud publishing is enabled|
|updateIntervalMinutes|How often (in minutes) to update current measurement data to cloud|
|historyUpdateIntervalMinutes|History data publish interval in minutes. Currently this is not very useful, because history data is aggregated for each hour|
|historyLengthDays|How many days of measurement history to publish|
|firebaseStorage|Configuration of Firebase database|

#### FirebaseStorage
Google Firebase access details

|Property|Purpose|
|--------|-------|
|enabled|Indicates if Firebase storage is enabled|
|url|URL of the Firebase database where data is stored|
|databaseSecret|Database secret used to access database. Is found in `Project settings\Service accounts\Database secrets` in Firebase Console|

### AlertConfiguration
Alert configuration defines the alerts to be sent and how are they sent. If alerts are not needed, the section can be removed from configuration file.

|Property|Purpose|
|--------|-------|
|azurePushNotifications|Configuration of Azure push notifications|
|emailNotifications|Configuration of email notifications|
|alerts|Configuration of alerts|

#### AzurePushNotifications
Configuration for notifications sent using Azure Notification Hub. Requires setting up Azure account and notification hub.
|Property|Purpose|
|--------|-------|
|Enabled|Indicates if Azure push notifications are enabled|
|connectionString|Full access connection string to access Azure Notification Hub|
|notificationHubName|Name of the Azure Notification Hub|

#### EmailNotifications
Send email notifications using SMTP
|Property|Purpose|
|--------|-------|
|enabled|Indicates if email notifications are enabled|
|sender|Sender of the email|
|recipients|Recipients of the email notification|
|smtpServerAddress|Address of the SMTP server|
|smtpServerPort|Port of the SMTP server|
|authentication|SMTP authentication configuration|

##### SmtpAuthentication
Authentication configuration related to SMTP server. Set `enabled` to `false` if SMTP server does not require authentication.
|Property|Purpose|
|--------|-------|
|enabled|Indicates if SMTP authentication is enabled|
|userName|User name to connect the SMTP server|
|password|Password to connect the SMTP server|

#### Alerts
Contains list of alerts
|Property|Purpose|
|--------|-------|
|id|Unique id of the alert|
|description|Description for alert|
|title|Title/subject of the notification message to be sent|
|message|Body text of the notification message to be sent|
|ruleSet|The rules to define when alert notification needs to be sent|

##### RuleSet
`type` defines how rules are handled. Two options for value are `AND` and `OR`. Earlier means that all rules must be true before sending alert and latter means that alert is send when any of the rules is true.

Three operator types are supported: `AreEqualTo`, `IsGreaterThan` and `IsLessThan`
|Property|Purpose|
|--------|-------|
|location|Location where sensor is|
|sensorDeviceId|ID of the sensor device|
|sensorId|ID of the sensor|
|operator|Operator type|
|value|Value to compare against>

Example:
```
"dataBrokerConfiguration": {
    "dataBrokerEnabled": true,
    "mqttServerAddress": "192.168.1.2",
    "updateIntervalMinutes": "5",
    "cloudPublishConfiguration": {
      "Enabled": true,
      "publishUrl": "https://xyz.firebaseio.com/vahti/",
      "historyUpdateIntervalMinutes": "30",
      "historyLengthDays": "3"
    },    
    "alertConfiguration": {
      "azurePushNotifications": {
        "enabled": true,
        "connectionString": "Endpoint=sb://myhub.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=xxx",
        "notificationHubName": "MyHubName"
      },
      "emailNotifications": {
        "Enabled": true,
        "Sender": "John Doe <john.doe@domain.com>",
        "Recipients": [
          "John Doe <john.doe@domain.com>",
          "jane.doe@domain.com"
        ],
        "smtpServerAddress": "smtpa.mailserver.com",
        "smtpServerPort": "465",
        "smtpAuthentication": {
          "enabled": true,
          "UserName": "myusername",
          "Password": "mystrongpassword"
        }
      },
      "alerts": [
        {
          "id": "mowerMissing",
          "description": "Mower is missing",
          "title": "Alert",
          "message": "Mower has not been in charger for an hour",
          "ruleSet": {
            "type": "AND",
            "rules": [
              {
                "location": "Mower",
                "sensorDeviceId": "ruuviMower",
                "sensorId": "lastCharged",
                "operator": "IsGreaterThan",
                "value": "3600"
              }
            ]
          }
        },
        {
          "id": "garageTempLow",
          "description": "Garage temp is too low",
          "title": "Alert",
          "message": "Garage temp is under 15°C",
          "ruleSet": {
            "type": "AND",
            "rules": [
              {
                "location": "Garage",
                "sensorDeviceId": "ruuviGarage",
                "sensorId": "temperature",
                "operator": "IsLessThan",
                "value": "15"
              }
            ]
          }
        }
      ]
    }
  }
```

### BluetoothGwConfiguration
Bluetooth gateway is responsible for reading data from Bluetooth LE devices and publish the data using MQTT. Disable it if you don't have Bluetooth LE devices that you would like to monitor using this service.

|Property|Purpose|
|--------|-------|
|bluetoothGwEnabled|Indicates if Bluetooth gateway service is enabled|
|mqttServerAddress|Address of the MQTT server|
|scanIntervalSeconds|How often to read data from Bluetooth LE devices|
|adapterName|Name of Bluetooth adapter used when scanning|
|sensorDeviceTypes|List of sensor device types|
|sensorDevices|List of devices having sensors| 

### SensorDeviceType
Defines the sensors that a sensor device has. One sensor device can have multiple sensors (like temperature and humidity).

|Property|Purpose|
|--------|-------|
|id|Unique ID of the sensor device type|
|name|Name of sensor device type. Can be shown in user interfaces|
|manufacturer|Name of sensor manufacturer|
|sensors|List of sensors that the device has|

#### Sensor
Represents one sensor of a sensor device.

|Property|Purpose|
|--------|-------|
|id|Unique ID of the sensor|
|name|Name of the sensor that could be shown in user interfaces|
|class|Indicates to which sensor class sensor belongs to. Mobile app UI shows the data based on their class|
|unit|Unit of the measurement value. Used in mobile app user interface.

#### Class of a sensor
Sensor classes used in configuration file are mapped against enum values defined in [SensorClass](..\src\Vahti.Shared\Enum\SensorClass.cs). In addition, when adding new sensor classes, the mobile app UI must be updated so that it knows how to display it in UI.

|Property|Purpose|
|--------|-------|
|temperature|Temperature|
|humidity|Humidity|
|pressure|Air pressure|
|accelerationX|Acceleration (X)|
|accelerationY|Acceleration (Y)|
|accelerationZ|Acceleration (Z)|
|batteryVoltage|Voltage of battery|
|rssi|RSSI (Received Signal Strength Indication)|
|movementCounter|How many times something has moved|
|lastCharged|Indicates when something was charged last time (I use it for my robotic mower)

### SensorDevice
Represents a single instance of an sensor device type.

|Property|Purpose|
|--------|-------|
|id|Unique ID of the sensor device instance|
|name|Name of the sensor device. Could be used in user interfaces|
|sensorDeviceTypeId|ID of the sensor device type|
|address|Address information (could be MAC address for Bluetooth LE devices)|
|location|Name of location to which the sensor is related to. Location name is also shown in mobile app UI|
|calculatedMeasurements|List of sensor device instance specific custom measurements|

#### CalculatedMeasurement
An experimental way to support sensor device instance specific measurements. In my case I use RuuviTag acceleration sensor to monitor when my robotic mower is in charger, and the value is how many seconds has passed since it condition was true last time.

|Property|Purpose|
|--------|-------|
|id|Unique ID of the measurement|
|name|Name of the measurement. Could be shown in user interfaces|
|class|[SensorClass](..\src\Vahti.Shared\Enum\SensorClass.cs) of the measurement|
|unit|Unit of the measurement value. Shown in mobile app UI|
|type|Type of the measurement. `default` means regular condition (e.g. temperature is less than 20) and `wasLast` means when condition was true last time|
|sensorId|ID of the sensor|
|operator|Operator type|
|value|Value to compare against|


  ```
  "bluetoothGwConfiguration": {
    "bluetoothGwEnabled": true,
    "mqttServerAddress": "192.168.1.2",
    "scanIntervalSeconds": "60",
    "adapterName": "hci0",
    "sensorDevices": [
      {
        "id": "ruuviGarage",
        "address": "12:34:56:78:90:AB",
        "name": "RuuviTag in garage",
        "sensorDeviceTypeId": "RuuviTag",
        "location": "Garage"
      },
      {
        "id": "ruuviMower",
        "address": "AB:90:78:56:34:12",
        "name": "RuuviTag on robotic mower charger",
        "sensorDeviceTypeId": "RuuviTag",
        "location": "Mower",
        "calculatedMeasurements": [
          {
            "id": "lastCharged",
            "name": "Last charged",
            "class": "lastCharged",
            "unit": "s",
            "type": "wasLast",
            "sensorId": "accelerationY",
            "operator": "isLessThan",
            "value": "-0.15"
          }
        ]
      }
    ],
    "sensorDeviceTypes": [
      {
        "id": "RuuviTag",
        "name": "RuuviTag",
        "manufacturer": "Ruuvi",
        "sensors": [
          {
            "id": "temperature",
            "name": "Temperature",
            "class": "temperature",
            "unit": "°C"
          },
          {
            "id": "humidity",
            "name": "Relative humidity",
            "class": "humidity",
            "unit": "%"
          },
          {
            "id": "pressure",
            "name": "Air pressure",
            "class": "pressure",
            "unit": "hPa"
          },
          {
            "id": "accelerationX",
            "name": "Acceleration (X)",
            "class": "accelerationX",
            "unit": "G"
          },
          {
            "id": "accelerationY",
            "name": "Acceleration (Y)",
            "class": "accelerationY",
            "unit": "G"
          },
          {
            "id": "accelerationZ",
            "name": "Acceleration (Z)",
            "class": "accelerationZ",
            "unit": "G"
          },
          {
            "id": "batteryVoltage",
            "name": "Battery voltage",
            "class": "batteryvoltage",
            "unit": "V"
          },
          {
            "id": "movementCounter",
            "name": "Movement counter",
            "class": "movementCounter",
            "unit": ""
          }
        ]
      }
    ]
  }
  ```
### Logging
Optional logging section provide way to configure what information is logged in console. See [Logging in .NET Core and ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1) for more details

```
"Logging": {
    "LogLevel": {
      "Default": "Information",
      "System": "Information",
      "Microsoft": "Information"
    },
    "Console": {
      "IncludeScopes": true
    }
  }
```

