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
Read [Create Firebase database](CreateFirebaseDatabase.md) to get information on how to create Firebase database, where data is stored.

### Update mobile projects
1. Set Vahti.Mobile.Android project as start-up project in toolbar (or via project context menu)
2. Put your mobile device in developer mode: https://developer.android.com/studio/debug/dev-options 
3. Connect your mobile device to PC with USB cable
4. You mobile device should now appear as selectable device in toolbar
5. Build solution (CTRL-Shift-B)
6. Build should succeed and you can run the application, which deploy it in your mobile device

### Set up Raspberry Pi
Read [Setting up Raspberry Pi](SettingUpRaspberryPi.md) to get detailed information about needed Raspberry Pi setup.

Some notes regarding editing `config.json`:
- In scenario described in this tutorial, you need to have all services enabled and configured
- You can disable also `alertConfiguration` if you don't want to configure alerts yet
- Fill your database URL (and database secret if authentication is enabled) in `FirebaseStorage` section
- Update MQTT server address in `DataBrokerConfiguration` and `CollectorConfiguration` sections. As it's running on same device, use the IP of the Raspberry Pi
- Update `CollectorConfiguration` according to how many RuuviTags you have. `location` of SensorDevice is shown in mobile app UI, `address` is the MAC address of the RuuviTag and `name` is free text. You can update `name` of each sensor in `SensorDeviceType` to have localized text on your language
- Configure suitable interval for data publishing in `DataBrokerConfiguration`. I use 5 minutes (and 30 minutes for history data).