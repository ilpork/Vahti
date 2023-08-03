# FAQ
### Android app does not show push notification alerts as expected
- Check that you have configured the alerts as guided in tutorials (Vahti.Server/DataBroker, Google messaging and Azure notification hub)
- Check that you have `src/Vahti.Mobile/Platforms/Android/google-services.json` and `src/Vahti.Mobile/appsettings.json` (with proper information)
- Note that location of files mentioned above has changed since v1.x, and `appsettings.json` is now used instead of `App.config`

### I've built the app from sources and it still asks for database URL on start-up
- Check that you have `src/Vahti.Mobile/appsettings.json` with proper information (it used to be `src/Vahti.Mobile/Vahti.Mobile.Forms/App.config` in v1.x)