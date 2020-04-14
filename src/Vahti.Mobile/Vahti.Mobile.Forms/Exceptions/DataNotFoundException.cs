using Vahti.Shared.Exception;

namespace Vahti.Mobile.Forms.Exceptions
{
    /// <summary>
    /// Exception used when requested data was not found 
    /// </summary>
    public class DataNotFoundException : System.Exception
    {
        public string Url { get; set; }

        public DataNotFoundException(string url) : base() 
        {
            Url = url;
        }
    }
}
