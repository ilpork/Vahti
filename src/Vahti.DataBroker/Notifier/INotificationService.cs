using System.Threading.Tasks;

namespace Vahti.DataBroker.Notifier
{
    /// <summary>
    /// Defines notification sending related functionality 
    /// </summary>
    public interface INotificationService
    {
        bool Enabled { get; }
        Task Send(string title, string message);
    }
}
