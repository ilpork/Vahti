using System.Collections.Generic;
using System.Threading.Tasks;
using Vahti.Shared.Data;

namespace Vahti.DataBroker.AlertHandler 
{ 
    /// <summary>
    /// Defines functionality related to sending alerts
    /// </summary>
    public interface IAlertHandler
    {
        Task<bool> Send(List<LocationData> locations);
    }
}
