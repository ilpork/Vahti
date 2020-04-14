namespace Vahti.DataBroker.Configuration
{
    /// <summary>
    /// Represents configuration of Azure push notifications
    /// </summary>
    public class AzurePushNotificationConfiguration
    {
        public virtual bool Enabled { get; set; }
        public virtual string ConnectionString { get; set; }
        public virtual string NotificationHubName { get; set; }
    }
}
