namespace Vahti.Mobile.EventArguments
{
    /// <summary>
    /// Event arguments used when app is resumed
    /// </summary>
    public class AppResumedEventArgs : EventArgs
    {
        public Page CurrentPage { get; }       

        public AppResumedEventArgs(Page currentPage)
        {
            CurrentPage = currentPage;
        }
    }
}
