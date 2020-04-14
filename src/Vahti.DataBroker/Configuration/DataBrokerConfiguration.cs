namespace Vahti.DataBroker.Configuration
{
    /// <summary>
    /// Represents configuration of <see cref="DataBrokerService"/>
    /// </summary>
    public class DataBrokerConfiguration
    {
        public virtual bool DataBrokerEnabled { get; set; }
        public virtual string MqttServerAddress { get; set; }        
        public virtual CloudPublishConfiguration CloudPublishConfiguration { get; set; }
        public virtual AlertConfiguration AlertConfiguration { get; set; }        
    }
}
