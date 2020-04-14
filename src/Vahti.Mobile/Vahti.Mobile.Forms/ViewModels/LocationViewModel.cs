using Vahti.Mobile.Forms.Services;
using Vahti.Mobile.Forms.EventArguments;
using Vahti.Mobile.Forms.Models;

namespace Vahti.Mobile.Forms.ViewModels
{
    /// <summary>
    /// View model for page having tabs to show graph and details of sensors
    /// </summary>
    public class LocationViewModel : BaseViewModel
    {
        public LocationViewModel(INavigationService navigationService) : base(navigationService)
        {
            NavigationService.NavigatedTo += NavigationService_NavigatedTo;
        }

        private void NavigationService_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            if (e.Page == Constants.PageType.Location)
            {
                Title = ((Location)e.Parameter).Name;
            }
        }
    }
}
