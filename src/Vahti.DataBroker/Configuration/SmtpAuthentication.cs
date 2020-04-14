using System;
using System.Collections.Generic;
using System.Text;

namespace Vahti.DataBroker.Configuration
{
    public class SmtpAuthentication
    {
        public virtual bool Enabled { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
    }
}
