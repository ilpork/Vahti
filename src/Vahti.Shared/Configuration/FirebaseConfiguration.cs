
namespace Vahti.Shared.Configuration
{
    /// <summary>
    /// Represents configuration of Firebase data storage settings
    /// </summary>
    public class FirebaseConfiguration
    {
        public virtual bool Enabled { get; set; }
        public virtual string Url { get; set; }
        public virtual string DatabaseSecret { get; set; }
    }
}
