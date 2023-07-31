using Vahti.Mobile.Constants;
using NavigatedToEventArgs = Vahti.Mobile.EventArguments.NavigatedToEventArgs;

namespace Vahti.Mobile.Services
{
    /// <summary>
    /// Service used for navigation between pages outside the scope of <see cref="Shell"/>
    /// </summary>
    public class NavigationService : INavigationService
    {
        public event EventHandler<NavigatedToEventArgs> NavigatedTo;

        public async Task NavigateTo(PageType pageType, object parameter = null)
        {
            string routeId;
            switch (pageType)
            {
                case PageType.Location:
                    routeId = "location";
                    break;
                case PageType.Options:
                    routeId = "options";
                    break;
                case PageType.Details:
                    routeId = "location/details";
                    break;
                default:
                    throw new NotImplementedException(nameof(pageType));
            }
            NavigatedTo?.Invoke(this, new NavigatedToEventArgs(pageType, parameter));
            await Shell.Current.GoToAsync(routeId);
        }
    }
}
