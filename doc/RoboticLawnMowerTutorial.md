# Tutorial for monitoring a robotic lawn mower
Why I originally started to develop early version of this system was to check if my robotic lawn mower has returned safely back to its charging station after its work session. There are some obstacles in our garden, and sometimes the mower gets stuck on them. So I wanted to get notification to my phone if mower is not back in charging station in one hour.

Charging station of Husqvarna Automower (as well as many other models) has a horizontal flap, which raises a bit when mower comes to there. RuuviTag needs to be put on that flap. Then you can investigate RuuviTag accelerator meter values to see which axis provides clearest change in sensor value when angle of the flap changes.

The following configuration is based on [Alert tutorial](AlertTutorial.md) for simplicity, because the main difference is in the configuration file. You can check other tutorials regarding how to set up mobile app. Mobile app shows the status of the mower (is it charging or how long has it been away). See screenshot on main README for an example.

In configuration file you need to define `calculatedMeasurements` for the SensorDevice. In this case it defines sensor with class `lastCharged`, so mobile app knows how to show the data in user interface. Other fields define that value of `lastCharged` is the time passed since given condition was true last time.

Alert respectively defines that alert is sent when the value is greater than 1 hour (3600 seconds). 

This tutorial assumes that you have Raspberry Pi (at 192.168.1.2) and you want to get notifications by email.

For basic setup of Raspberry Pi you can read [Setting up Raspberry Pi](SettingUpRaspberryPi.md).

1. Create new directory: `mkdir ~/vahti && cd ~/vahti`
2. Download Vahti.Server binaries: `wget https://github.com/ilpork/vahti/releases/latest/download/Vahti.Server-linux-arm.zip`
3. Extract the files: `unzip Vahti.Server-linux-arm.zip`
4. Copy `config.template.json` as `config.json` and modify it to suit your configuration (like email settings and RuuviTag address). See [Configuration](Configuration.md) for details.
4. Run the app with `./Vahti.Server`

## Example configuration
This example assumes that you have one RuuviTag and it's on the mower.
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
          "id": "mower",
          "description": "Mower has not returned",
          "title": "Alert",
          "message": "Mower has not returned back to charging station",
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
        "id": "ruuviMower",
        "address": "12:34:56:78:90:AB",
        "name": "RuuviTag on mower",
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
            "value": "-0.1"
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
}
```
