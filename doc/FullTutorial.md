# Tutorial for full setup
Tutorial is assumes that you have a Raspberry Pi, run all needed services on that and use Android mobile app. You have two RuuviTag sensors, which data you want to get, and you already know their MAC addresses. This also assumes that you build the mobile application yourself. 

The tutorial contains a lot of steps, but the aim is to help those who are not so familiar with setting up systems like this.

### Install Visual Studio 2019 (Community edition is fine) with mobile development options needed 
1. Install Visual Studio 2019 community https://visualstudio.microsoft.com/vs/community/
2. In Visual Studio installer select "Mobile development with .NET" and ".NET Core cross-platform development". In addition, select also Git options from "individual components" tab
3. When installed, start Visual Studio
4. Select "Clone or check out code"
5. Type "https://github.com/ilpork/Vahti" in 'Repository location"
6. Select Path where to clone the source code repository
7. Clone
8. Open 'src' folder in Solution explorer and open 'Vahti.sln'
9. Accept license for Android SDK and related components. While waiting for components to be installed, move on to creating Azure account and Google Firebase project

### Create Azure account and a Notification Hub
- Microsoft has a good tutorial about how to create the Azure notification hub and how to link it to Google Firebase. Follow the tutorial until Xamarin app creation starts (you don't need to make code changes in Mobile app). Use `com.ilpork.vahti` for package name and place the config file (google-services.json) in root of the Vahti.Mobile.Android project

 https://docs.microsoft.com/en-us/azure/notification-hubs/xamarin-notification-hubs-push-notifications-android-gcm

### Create Google Firebase realtime database
1. In Google Firebase project overview page select "Database" and "Create realtime database". Select "Start in test mode" to use it without authentication 
2. Now you see the URL of your database and copy it to clipboard
3. If App.config does not already exist in root of Vahti.Mobile.Android project, copy App.template.config as App.config
4. Put the database URL in `value` field of `FirebaseDatabaseUrl` app setting in `App.config`
5. When you've later noticed that everything works, you can enable authentication for Firebase database (in `Rules` tab of Firebase database console) and put database secret (found in `Project settings\Service accounts\Database secrets` in Firebase Console) as `value` of `FirebaseDatabaseSecret` app setting

### Update mobile projects
1. Set Vahti.Mobile.Android project as start-up project in toolbar (or via project context menu)
2. Put your mobile device in developer mode: https://developer.android.com/studio/debug/dev-options 
3. Connect your mobile device to PC with USB cable
4. You mobile device should now appear as selectable device in toolbar
5. Build solution (CTRL-Shift-B)
6. Build should succeed and you can run the application, which deploy it in your mobile device

### Set up the Raspberry Pi
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
12. Download Vahti.Server binaries: `wget https://github.com/ilpork/vahti/releases/latest/download/Vahti.Server-linux-arm.zip`
13. Extract the files: `tar -xzf Vahti.Server.zip`
14. Move to dir
15. Copy config.template.json as config.json 'cp config.template.json config.json'
16. Update user permission on config.json so that only user `pi` can read it: `chmod 600 config.json`
17. Edit config.json: 'nano config.json'. See [Configuration](Configuration.md) for details on how to configure the app.
- In scenario described in this tutorial, you need to have all services enabled and configured
- You can disable `alertConfiguration` if you don't want to configure alerts yet
- Fill your database URL (and database secret if authentication is enabled) in `firebaseStorage` section
- Update MQTT server address in `DataBrokerConfiguration` and `BluetoothGwConfiguration` sections. As it's running on same device, use the IP of the Raspberry Pi
- Update `BluetoothGwConfiguration` according to how many RuuviTags you have. `location` of SensorDevice is shown in mobile app UI, `address` is the MAC address of the RuuviTag and `name` is free text. You can update `name` of each sensor in `SensorDeviceType` to have localized text on your language
- Configure suitable interval for data publishing in `DataBrokerConfiguration`. I use 5 minutes (and 30 minutes for history data).
18. Add user to bluetooth group 'sudo adduser pi bluetooth'
19. Reboot 'sudo reboot now'
20. Now you can navigate to folder and start the app by executing `./Vahti.Server`
