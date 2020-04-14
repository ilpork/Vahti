using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.MailKitSmtp;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Vahti.DataBroker.Configuration;

namespace Vahti.DataBroker.Notifier
{
    /// <summary>
    /// Provides functionality to send notifications to users
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly AlertConfiguration _config;        

        public NotificationService(IOptions<DataBrokerConfiguration> config)
        {            
            _config = config.Value.AlertConfiguration;
        }

        public bool Enabled => _config != null && ((_config.AzurePushNotifications != null && _config.AzurePushNotifications.Enabled) || 
            (_config.EmailNotifications != null && _config.EmailNotifications.Enabled));

        public async Task Send(string title, string message)
        {
            // Send push notification
            if (_config.AzurePushNotifications != null && _config.AzurePushNotifications.Enabled)
            {
                var hub = NotificationHubClient.CreateClientFromConnectionString(_config.AzurePushNotifications.ConnectionString, 
                    _config.AzurePushNotifications.NotificationHubName);
                var payload = JsonConvert.SerializeObject(new AzureNotificationPayload(title, message));
                await hub.SendFcmNativeNotificationAsync(payload);
            }
            // Send email notification
            if (_config.EmailNotifications != null && _config.EmailNotifications.Enabled)
            {
                var emailConfig = _config.EmailNotifications;

                if (emailConfig.Recipients != null)
                {
                    var authenticationEnabled = emailConfig.SmtpAuthentication != null &&
                        emailConfig.SmtpAuthentication.Enabled;

                    var smtpClientOptions = new SmtpClientOptions()
                    {
                        Server = emailConfig.SmtpServerAddress,
                        Port = emailConfig.SmtpServerPort,
                        UseSsl = authenticationEnabled,
                        RequiresAuthentication = authenticationEnabled,
                    };
                    
                    if (authenticationEnabled)
                    {
                        smtpClientOptions.User = emailConfig.SmtpAuthentication.UserName;
                        smtpClientOptions.Password = emailConfig.SmtpAuthentication.Password;                        
                    }
                                        
                    Email.DefaultSender = new MailKitSender(smtpClientOptions);
                    
                    var recipientAddresses = _config.EmailNotifications.Recipients.Select(r => 
                    {
                        var address = new MailAddress(r);
                        return new Address(address.Address, address.DisplayName);
                    }
                    );
                    var senderAddress = new MailAddress(_config.EmailNotifications.Sender);
                    var email = await Email.From(senderAddress.Address, senderAddress.DisplayName)
                        .To(recipientAddresses.ToList())
                        .Subject(title)
                        .Body(message)
                        .SendAsync();                     
                }                
            }
        }

        private class AzureNotificationPayload
        {
            public AzureNotificationPayload(string title, string message)
            {
                Data = new AzureNotificationData() { Message = message, Title = title };
            }

            public AzureNotificationData Data { get; }
        }

        private class AzureNotificationData
        {
            public string Message { get; set; }
            public string Title { get; set; }
        }
    }

}
