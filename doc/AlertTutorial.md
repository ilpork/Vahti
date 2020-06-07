# Tutorial for alert setup
Setting up Vahti is simpler if you don't want to use the mobile app. This tutorial assumes that you have Raspberry Pi (at 192.168.1.2) and you want to get notifications as email.

For basic setup of Raspberry Pi you can read [Setting up Raspberry Pi](SettingUpRaspberryPi.md)

1. Create new directory: `mkdir ~/vahti && cd ~/vahti`
2. Download Vahti.Server binaries: `wget https://github.com/ilpork/vahti/releases/latest/download/Vahti.Server-linux-arm.zip`
3. Extract the files: `unzip Vahti.Server-linux-arm.zip`
4. Copy `config.template.json` as `config.json` and modify it to suit your configuration. See [Configuration](Configuration.md) for details.
4. Run the app `./Vahti.Server`

## Example configuration 1
This example assumes that you have one RuuviTag. Change logging level to `Information` or `Debug` to get more detailed output.
```
{
  "mqttConfiguration": {
    "mqttServerEnabled": true
  },
  "dataBrokerConfiguration": {
    "dataBrokerEnabled": true,
    "mqttServerAddress": "192.168.1.2",
    "cloudPublishConfiguration": {
      "Enabled": false      
    },    
    "alertConfiguration": {  
      "enabled": true,    
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
  },
  "collectorConfiguration": {
    "collectorEnabled": true,
    "mqttServerAddress": "192.168.1.2",
    "scanIntervalSeconds": "60",
    "bluetoothAdapterName": "hci0",
    "sensorDevices": [
      {
        "id": "ruuviGarage",
        "address": "12:34:56:78:90:AB",
        "name": "RuuviTag in garage",
        "sensorDeviceTypeId": "RuuviTag",
        "location": "Garage"
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
  },
  "Logging": {
    "LogLevel": {
      "Vahti": "Warning",
      "Default": "Information",
      "System": "Information",
      "Microsoft": "Information"
    },
    "Console": {
      "IncludeScopes": true
    }
  }
}
```

## Example configuration 2
If you don't have RuuviTag, DHT22 or other device supported by Vahti.Collector, but some other source providing data (see [Add other data sources](AddOtherDataSources.md)), the configuration file is even simpler. This example assumes that you have configured a sensor device with name `dhtGarage` to publish temperature data.

Change logging level of Vahti to `Information` or `Debug` to get more detailed output.

```
{
  "mqttConfiguration": {
    "mqttServerEnabled": true
  },
  "dataBrokerConfiguration": {
    "dataBrokerEnabled": true,
    "mqttServerAddress": "192.168.1.2",
    "cloudPublishConfiguration": {
      "Enabled": false      
    },    
    "alertConfiguration": {      
      "enabled": true,
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
          "id": "garageTempLow",
          "description": "Garage temp is too low",
          "title": "Alert",
          "message": "Garage temp is under 15°C",
          "ruleSet": {
            "type": "AND",
            "rules": [
              {
                "location": "Garage",
                "sensorDeviceId": "dhtGarage",
                "sensorId": "temperature",
                "operator": "IsLessThan",
                "value": "15"
              }
            ]
          }
        }
      ]
    }
  },
  "Logging": {
    "LogLevel": {
      "Vahti": "Warning",
      "Default": "Information",
      "System": "Information",
      "Microsoft": "Information"
    },
    "Console": {
      "IncludeScopes": true
    }
  }
}
```

