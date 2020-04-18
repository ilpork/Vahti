### Creating Google Firebase realtime database
Do the first two steps if you haven't setup Google project yet
1. Login to Google Firebase with Google account
2. Create a project
3. In Google Firebase project overview page select "Database" and "Create realtime database". Select "Start in test mode" to use it without authentication 
4. Now you see the URL of your database and copy it to clipboard

If you build mobile app yourself (setting up Azure notifications), it's good to put the database URL and secret in mobile app configuration file like instructed below, so you don't need to type those in mobile app. If you just use pre-compiled app, you must set the values on Options page of the app.

5. If App.config does not already exist in root of Vahti.Mobile.Android project, copy App.template.config as App.config
6. Put the database URL in `value` field of `FirebaseDatabaseUrl` app setting in `App.config`
7. When you've later noticed that everything works, you can enable authentication for Firebase database (in `Rules` tab of Firebase database console) and put database secret (found in `Project settings\Service accounts\Database secrets` in Firebase Console) as `value` of `FirebaseDatabaseSecret` app setting