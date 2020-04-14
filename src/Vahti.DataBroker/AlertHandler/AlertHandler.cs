using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vahti.DataBroker.Configuration;
using Vahti.DataBroker.DataProvider;
using Vahti.DataBroker.Notifier;
using Vahti.Shared.Data;
using Vahti.Shared.Enum;
using Vahti.Shared.Utils;

namespace Vahti.DataBroker.AlertHandler
{
    /// <summary>
    /// Provides functionality to send alerts
    /// </summary>
    public class AlertHandler : IAlertHandler
    {
        private readonly INotificationService _notificationService;
        private readonly INoticationDataProvider _noticationDataProvider;
        private readonly ILogger<AlertHandler> _logger;
        private readonly DataBrokerConfiguration _config;

        public AlertHandler(ILogger<AlertHandler> logger, INotificationService notificationService, INoticationDataProvider notificationDataProvider,
            IOptions<DataBrokerConfiguration> dataBrokerConfig)
        {
            _notificationService = notificationService;
            _noticationDataProvider = notificationDataProvider;
            _logger = logger;
            _config = dataBrokerConfig.Value;
        }

        public async Task<bool> Send(List<LocationData> locations)
        {
            if (!_config.AlertConfiguration.Enabled)
            {
                return false;    
            }                

            foreach (var alert in _config.AlertConfiguration.Alerts)
            {
                bool alertNeedsToBeSent = false;
                foreach (var rule in alert.RuleSet.Rules)
                {
                    var location = locations.FirstOrDefault(l => l.Id.Equals(rule.Location, StringComparison.OrdinalIgnoreCase));
                    var measurement = location?.Measurements.FirstOrDefault(m => m.SensorDeviceId.Equals(rule.SensorDeviceId, StringComparison.OrdinalIgnoreCase) &&
                        m.SensorId.Equals(rule.SensorId, StringComparison.OrdinalIgnoreCase));

                    if (measurement == null)
                    {
                        continue;
                    }

                    var comparisonResult = LogicHelper.Compare(measurement.Value, rule.Value, rule.Operator);

                    if (!comparisonResult && alert.RuleSet.Type == MeasurementRuleSetType.AND)
                    {
                        alertNeedsToBeSent = false;
                        break;
                    }
                    else if (!comparisonResult && alert.RuleSet.Type == MeasurementRuleSetType.OR)
                    {
                        continue;
                    }
                    else if (comparisonResult && alert.RuleSet.Type == MeasurementRuleSetType.AND)
                    {
                        alertNeedsToBeSent = true;
                        continue;
                    }
                    else if (comparisonResult && alert.RuleSet.Type == MeasurementRuleSetType.OR)
                    {
                        alertNeedsToBeSent = true;
                        break;
                    }
                }
                if (alertNeedsToBeSent)
                {
                    var hasNotificationBeenSent = await _noticationDataProvider.HasNotificationBeenSent(alert.Id);
                    if (!hasNotificationBeenSent)
                    {
                        await _notificationService.Send(alert.Title, alert.Message);
                        await _noticationDataProvider.SetNotificationStatus(alert.Id, true);
                        _logger.LogInformation($"{DateTime.Now}: Sent notification of '{alert.Description}' ({alert.Id})");                        
                    }
                }
                else
                {
                    await _noticationDataProvider.SetNotificationStatus(alert.Id, false);
                }
            }
            return true;
        }
    }
}
