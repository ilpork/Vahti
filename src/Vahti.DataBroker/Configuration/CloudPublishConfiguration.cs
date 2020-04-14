using System.Collections.Generic;
using Vahti.Shared.Configuration;
using Vahti.Shared.TypeData;

namespace Vahti.DataBroker.Configuration
{
    /// <summary>
    /// Represents cloud publishing configuration
    /// </summary>
    public class CloudPublishConfiguration
    {
        public virtual bool Enabled { get; set; }
        public virtual int UpdateIntervalMinutes { get; set; }
        public virtual int HistoryUpdateIntervalMinutes { get; set; }
        public virtual int HistoryLengthDays { get; set; }
        public virtual FirebaseConfiguration FirebaseStorage { get; set; }

    }
}
