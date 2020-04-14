using System.Threading.Tasks;

namespace Vahti.DataBroker.DataProvider
{
    /// <summary>
    /// Defines functionality to access notification data in local database
    /// </summary>
    public interface INoticationDataProvider
    {
        public Task<bool> HasNotificationBeenSent(string alertId);
        public Task SetNotificationStatus(string alertId, bool isSent);
    }
}
