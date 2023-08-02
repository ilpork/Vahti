using Vahti.Mobile.Constants;

namespace Vahti.Mobile.EventArguments
{
    /// <summary>
    /// Event arguments used when navigating to a page
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
