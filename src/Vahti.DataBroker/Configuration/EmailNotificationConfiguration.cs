using System;
using System.Collections.Generic;
using System.Text;

namespace Vahti.DataBroker.Configuration
{
    /// <summary>
    /// Represents configuration of email notifications
    /// </summary>
    public class EmailNotificationConfiguration
    {
        public virtual bool Enabled { get; set; }
        public virtual string Sender { get; set; }
        public virtual List<string> Recipients { get; set; }
        public virtual string SmtpServerAddress { get; set; }
        public virtual int SmtpServerPort { get; set; }
        public virtual SmtpAuthentication SmtpAuthentication { get; set; }       
    }
}
