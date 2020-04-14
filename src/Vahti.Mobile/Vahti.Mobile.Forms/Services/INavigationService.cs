using Vahti.Mobile.Forms.Constants;
using Vahti.Mobile.Forms.EventArguments;
using System;
using System.Threading.Tasks;

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
