# Set up the Raspberry Pi
Steps below are minimum steps for simple setup, so you should harden the security when you've checked that everything works.
1. Get Balena Etcher https://www.balena.io/etcher/
2. Get Raspbian https://www.raspberrypi.org/downloads/raspbian/
3. Extract image
4. Flash it using Etcher
5. Create empty file named `ssh` on boot partition of SD card to enable SSH service
6. Put card to Raspberry Pi
7. Connect Raspberry Pi to LAN and power it on
8. Connect using SSH terminal (`ssh pi@raspberrypi`). Default password is `raspberry`.
9. Change default password for user `pi` by typing `passwd`
10. Update Raspbian (`sudo apt update` and `sudo apt upgrade`)
11. Check BlueZ version by running `bluetoothd -v`. It should output "5.50" or newer
12. Create new directory: `mkdir ~/vahti && cd ~/vahti`
13. Download Vahti.Server binaries: `wget https://github.com/ilpork/vahti/releases/latest/download/Vahti.Server-linux-arm.zip`
14. Extract the files: `unzip Vahti.Server-linux-arm.zip`
15. Copy config.template.json as config.json `cp config.template.json config.json`
16. Update user permission on `config.json` so that only user `pi` can read it: `chmod 600 config.json`
17. Edit the configuration file: `nano config.json`. See [Configuration](Configuration.md) for details on how to configure the app.
18. Add user to bluetooth group `sudo adduser pi bluetooth`
19. Reboot `sudo reboot now`
20. After Raspberry Pi is online again, connect to it again using SSH
20. Now you can navigate to folder and start the app by executing `cd ~/vahti && ./Vahti.Server`

How to make Vahti a service starting up on boot:
1. Create service file `sudo nano /etc/systemd/system/vahti.service` with following content. 
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
Uncomment the `RuntimeMaxSec=86400` line if you have stability issues with service/DBus. It restarts the service periodically (time in seconds). You can add similar line to bluetooth service (`sudo nano /etc/systemd/system/bluetooth.target.wants/bluetooth.service`) to periodically restart Bluetooth service. If you edit the service files later, remember to reload the daemon (`sudo systemctl daemon-reload`) or reboot.

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
