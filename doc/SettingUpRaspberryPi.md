# Set up the Raspberry Pi
Steps below are minimum steps for simple setup, so you should harden the security when you've checked that everything works. The following assumes you're using default user name `pi`, so replace it where needed
1. Download Raspberry Pi Imager and install it (https://www.raspberrypi.com/software/) 
2. Start Raspberry Pi Imager
3. Choose operating system (typically Raspberry Pi OS desktop or lite)
4. Choose the drive where to flash the image
5. Click the button with the cog to edit advanced settings. Enable SSH, choose preferred authentication method and configure the other settings (e.g. user name, host name, WLAN settings and locale) if needed
6. Write the image
7. Put the card to Raspberry Pi, connect it to LAN and power it on
8. Connect to the PI using SSH (using authentication method, user name and host name) defined earlier
10. Update the OS (`sudo apt update` and `sudo apt upgrade`)
11. Check BlueZ version by running `bluetoothd -v`. It should output "5.50" or newer
12. Create new directory: `mkdir ~/vahti && cd ~/vahti`
13. Download Vahti.Server binaries: `wget https://github.com/ilpork/vahti/releases/latest/download/Vahti.Server-linux-arm.zip`
14. Extract the files: `unzip Vahti.Server-linux-arm.zip`
15. Copy config.template.json as config.json `cp config.template.json config.json`
16. Update user permission on `config.json` so that only user you've defined can read it: `chmod 600 config.json`
17. Edit the configuration file: `nano config.json`. See [Configuration](Configuration.md) for details on how to configure the app.
18. Add user to bluetooth group `sudo adduser pi bluetooth` (where `pi` is the user name you are using)
19. Reboot `sudo reboot now`
20. After Raspberry Pi is online again, connect to it again using SSH
20. Now you can navigate to folder and start the app by executing `cd ~/vahti && ./Vahti.Server`

How to make Vahti a service starting up on boot:
1. Create service file `sudo nano /etc/systemd/system/vahti.service` with following content (remember to use correct user name instead of `pi`)
```
[Unit] 
Description=Vahti 
Requires=network.target network-online.target bluetooth.target 
After=network.target network-online.target bluetooth.target 

[Service] 
ExecStart=/home/pi/vahti/Vahti.Server 
WorkingDirectory=/home/pi/vahti 
StandardOutput=syslog 
StandardError=syslog 
SyslogIdentifier=Vahti 
Restart=always 
User=pi
#RuntimeMaxSec=86400

[Install] 
WantedBy=multi-user.target
```
Uncomment the `RuntimeMaxSec=86400` line if you have stability issues with service/DBus. It restarts the service periodically (time in seconds). You can add similar line for bluetooth service (`sudo nano /etc/systemd/system/bluetooth.target.wants/bluetooth.service`) to periodically restart Bluetooth service. If you edit the service files later, remember to reload the daemon (`sudo systemctl daemon-reload`) or reboot.

2. Enable the service:
`sudo systemctl enable vahti`

3. Restart Raspberry Pi to check that service starts up correctly
`sudo reboot now`

4. Check that service has started: `systemctl status vahti`
```
vahti.service - Vahti
   Loaded: loaded (/etc/systemd/system/vahti.service; enabled; vendor preset: enabled)
   Active: active (running) since Sun 2020-06-07 12:27:08 EEST; 35min ago
 Main PID: 1210 (Vahti.Server)
    Tasks: 17 (limit: 4915)
   Memory: 27.9M
   CGroup: /system.slice/vahti.service
           └─1210 /home/pi/vahti/Vahti.Server
```
5. Logs can be found from Syslog. Example: `sudo journalctl -u vahti`
