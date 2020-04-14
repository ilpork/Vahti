# Add other data sources
Vahti uses MQTT to communicate and publish data. It means that any system can publish data that Vahti can use, and data can be then shown in mobile app.

It just requires the data is published in specified way. See [Configuration](Configuration.md) for details about how SensorDeviceType and SensorDevice must be defined.

1. Sender must publish sensor device type in format `sensordevicetype/mySensorDeviceType`
2. Sender must publish sensor device (instance) in format `sensordevice/mySensorDevice`
3. Sender must publish the measurement data in format `measurement/location/mySensorDevice/mySensorId`

## Example
[This Python script](..\src\Misc\publish_dht22.py) uses Eclipse Paho MQTT client library to publish data from DHT22 sensor attached to Raspberry Pi. 
```python
import Adafruit_DHT
import paho.mqtt.client as mqtt
import time
import json
import signal
import sys

DHT_SENSOR = Adafruit_DHT.DHT22 # Type of Adafruit sensor
DHT_PIN = 23 # Data pin used
READ_INTERVAL = 60 # Interval in seconds to read the values from sensor
SERVER_ADDRESS = "192.168.1.2" # Address of the Vahti.Server
CLIENT_NAME = "garage" # MQTT messages sent from here appear on server with this client name
SENSOR_DEVICE_TYPE = { "id": "dht22", "name": "DHT22", "manufacturer": "", "sensors": [ { "id": "temperature", "name": "Temperature", "class": "temperature", "unit": "°C" }, { "id": "humidity", "name": "Relative humidity", "class": "humidity", "unit": "%" } ] }
SENSOR_DEVICE_INSTANCE = { "id": "dhtGarage", "address": "23", "name": "Garage DHT22", "sensorDeviceTypeId": "dht22", "location": "Garage" }

# Publish device type and instance data when connected to server
def on_connect(client, userdata, flags, rc):
    if rc==0:
        client.connected_flag=True
        client.publish("sensordevicetype/dht22", json.dumps(SENSOR_DEVICE_TYPE))
        client.publish("sensordevice/dhtGarage", json.dumps(SENSOR_DEVICE_INSTANCE))
        print("Connected to server")
    else:
        print("Bad connection:", rc)

# Clean up when CTRL-C has been pressed
def signal_handler(sig, frame):
    cleanup()
    sys.exit(0)

# Disconnect from server when cleaning up
def cleanup():
    client.loop_stop()
    client.disconnect()
    print("Disconnected from server")

signal.signal(signal.SIGINT, signal_handler)

# Instantiate MQTT client and connect to server
mqtt.Client.connected_flag=False
client = mqtt.Client(CLIENT_NAME)
client.on_connect=on_connect
client.loop_start()

print("Connecting to server {0}...".format(SERVER_ADDRESS))
client.connect(SERVER_ADDRESS)

# Wait until connected
while not client.connected_flag:
    time.sleep(1)

# Loop until interrupted
while True:
    humidity, temperature = Adafruit_DHT.read_retry(DHT_SENSOR, DHT_PIN)
    if humidity is not None and temperature is not None:
        print("Temperature={0:0.1f}*C  Humidity={1:0.1f}%".format(temperature, humidity))
        client.publish("measurement/Garage/dhtGarage/temperature", temperature)
        client.publish("measurement/Garage/dhtGarage/humidity", humidity)
    else:
        print("Failed to retrieve data from the sensor")
    time.sleep(READ_INTERVAL)
```