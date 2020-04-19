# Set up the Raspberry Pi
Steps below are minimum steps for simple setup, so you should harden the security when you've checked that everything works.
1. Get Balena Etcher https://www.balena.io/etcher/
2. Get Raspbian https://www.raspberrypi.org/downloads/raspbian/
3. Extract image
4. Flash it using Etcher
5. Create empty file named `ssh` on boot partition of SD card to enable SSH service
6. Put card to Raspberry Pi
7. Connect Raspberry Pi to LAN and power it on
8. Connect using SSH terminal (`ssh pi@raspberrypi`) 
9. Change default password for user pi by typing `passwd`
10. Update Raspbian (`sudo apt update` and `sudo apt upgrade`)
11. Check BlueZ version by running `bluetoothd -v`. It should output "5.50" or newer
12. Create new directory: `mkdir ~/vahti && cd ~/vahti`
13. Download Vahti.Server binaries: `wget https://github.com/ilpork/vahti/releases/latest/download/Vahti.Server-linux-arm.zip`
14. Extract the files: `unzip Vahti.Server-linux-arm.zip`
15. Copy config.template.json as config.json `cp config.template.json config.json`
16. Update user permission on `config.json` so that only user `pi` can read it: `chmod 600 config.json`
17. Edit the configuration file: `nano config.json`. See [Configuration](Configuration.md) for details on how to configure the app.
- In scenario described in this tutorial, you need to have all services enabled and configured
- You can disable `alertConfiguration` if you don't want to configure alerts yet
- Fill your database URL (and database secret if authentication is enabled) in `firebaseStorage` section
- Update MQTT server address in `DataBrokerConfiguration` and `CollectorConfiguration` sections. As it's running on same device, use the IP of the Raspberry Pi
- Update `CollectorConfiguration` according to how many RuuviTags you have. `location` of SensorDevice is shown in mobile app UI, `address` is the MAC address of the RuuviTag and `name` is free text. You can update `name` of each sensor in `SensorDeviceType` to have localized text on your language
- Configure suitable interval for data publishing in `DataBrokerConfiguration`. I use 5 minutes (and 30 minutes for history data).
18. Add user to bluetooth group `sudo adduser pi bluetooth`
19. Reboot `sudo reboot now`
20. Now you can navigate to folder and start the app by executing `./Vahti.Server`