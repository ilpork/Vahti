# Tutorial for web application
This tutorial is assumes that you have already configured the server side and firebase database using the other tutorials.

At simplest, you can just open [vahti.netlify.app](https://vahti.netlify.app), tap the cog icon and specify the address of your data in "data source" field. This app will be running in Netlify at least for now. 

The address should be same as the address in configuration of `Vahti.Server` (dataBrokerConfiguration/cloudPublishConfiguration/firebaseStorage/url) with "/.json" added in the end. Example:

https://vahti-9c1dc.firebaseio.com/demo/.json

You can deploy the server also yourself. 
1. [Install Node, npm and ReactJS](https://reactjs.org/docs/create-a-new-react-app.html) 
2. Navigate to `src/Vahti.Web/Vahti.Web.ReactApp`
3. Type `npm install`
4. Type `npm start` to start the development server or
5. Type `npm run build` to build the production build

### Default data source
Default data source is read from environment variable `REACT_APP_DATA_SOURCE`. You can define it in shell or in [.env files](https://create-react-app.dev/docs/adding-custom-environment-variables/) in app root folder. If default data source is not defined, then app tells that it must be specified in settings page of the application.

### Deploying to app hosting services
At least [Netlify](https://netlify.com) supports deploying the app directly from GitHub, which is very easy. Just fork the repository, connect to it in Netlify and change few options in "Build & deploy" settings after creating the site:
1. Base directory: src/Vahti.Web/Vahti.Web.ReactApp
2. If you want to specify default data source, you can set environment variable `REACT_APP_DATA_SOURCE` in "Environment\Environment variables"
