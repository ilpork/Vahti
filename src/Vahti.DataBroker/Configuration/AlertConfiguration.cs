using System.Collections.Generic;
using Vahti.Shared.TypeData;

namespace Vahti.DataBroker.Configuration
{
    /// <summary>
    /// Represents configuration of alerts
    /// </summary>
    public class AlertConfiguration
    {        
        public virtual bool Enabled { get; set; }
        public virtual AzurePushNotificationConfiguration AzurePushNotifications { get; set; }
        public virtual EmailNotificationConfiguration EmailNotifications { get; set; }
        public virtual List<Alert> Alerts { get; set; }
    }
}
