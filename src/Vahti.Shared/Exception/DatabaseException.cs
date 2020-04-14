using System;

namespace Vahti.Shared.Exception
{
    /// <summary>
    /// Exception used accessing database failed
    /// </summary>
    public class DatabaseException : System.Exception
    {
        public string Url { get; set; }

        public DatabaseException(string url, System.Exception innerException) : base(null, innerException)
        {
            Url = url;
        }
    }
}
