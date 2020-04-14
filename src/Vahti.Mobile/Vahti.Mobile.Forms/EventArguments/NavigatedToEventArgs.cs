using Vahti.Mobile.Forms.Constants;

namespace Vahti.Mobile.Forms.EventArguments
{
    /// <summary>
    /// Event arguments used when navigating to a <see cref="Xamarin.Forms.Page"/>
    /// </summary>
    public class NavigatedToEventArgs : System.EventArgs
    {
        public PageType Page { get; }
        public object Parameter { get; }

        public NavigatedToEventArgs(PageType pageType, object parameter)
        {
            Page = pageType;
            Parameter = parameter;
        }
    }
}
