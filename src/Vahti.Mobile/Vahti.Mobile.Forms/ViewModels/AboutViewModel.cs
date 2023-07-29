using Vahti.Mobile.Forms.Services;
using System.Reflection;

namespace Vahti.Mobile.Forms.ViewModels
{
    /// <summary>
    /// View model for page used to show general information about application 
    /// </summary>
    public class AboutViewModel : BaseViewModel
    {
        public string Version { get; private set; }

        public AboutViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.About_Title;            
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }        
    }
}