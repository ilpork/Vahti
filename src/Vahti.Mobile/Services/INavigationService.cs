using Vahti.Mobile.Forms.Constants;
using NavigatedToEventArgs = Vahti.Mobile.Forms.EventArguments.NavigatedToEventArgs;

namespace Vahti.Mobile.Forms.Services
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
