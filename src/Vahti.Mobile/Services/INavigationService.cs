using Vahti.Mobile.Constants;
using NavigatedToEventArgs = Vahti.Mobile.EventArguments.NavigatedToEventArgs;

namespace Vahti.Mobile.Services
{
    /// <summary>
    /// Interface defining navigation functionality
    /// </summary>
    public interface INavigationService
    {
        event EventHandler<NavigatedToEventArgs> NavigatedTo;
        Task NavigateTo(PageType pageId, object parameter = null);
    }
}
