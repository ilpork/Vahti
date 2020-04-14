using System.Threading.Tasks;

namespace Vahti.DataBroker.DataBroker
{
    /// <summary>
    /// Defines functionality for handling and publishing gathered measurement data
    /// </summary>
    public interface IDataBroker
    {
        void MqttMessageReceived(string topic, string payload);
        Task<bool> PublishCurrentData();
        Task<bool> PublishHistoryData();
        Task<bool> DeleteOldHistoryData();
        Task<bool> SendAlerts();
    }
}