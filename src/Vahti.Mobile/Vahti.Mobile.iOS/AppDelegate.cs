
using Foundation;
using Vahti.Mobile.Forms.Theme;
using UIKit;
using Vahti.Mobile.Forms;

namespace Vahti.Mobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IThemeChanger
    {
        public void ApplyTheme(ColorThemeEnum theme)
        {
            
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental", "FastRenderers_Experimental");
            global::Xamarin.Forms.Forms.Init();
            OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();
            LoadApplication(new App(this));

            return base.FinishedLaunching(app, options);
        }
    }
}
