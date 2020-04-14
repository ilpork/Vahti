using Android.App;
using Android.OS;

namespace Vahti.Mobile.Droid
{
    /// <summary>
    /// Activity showing the splash screen on start-up
    /// </summary>
    [Activity(Label = "Vahti", Theme = "@style/Splash", MainLauncher = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
            Finish();
        }
    }
}