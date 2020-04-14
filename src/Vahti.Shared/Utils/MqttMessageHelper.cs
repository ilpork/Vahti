using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vahti.Shared.Utils
{
    /// <summary>
    /// Provides functionality to publish Vahti related MQTT messages in uniform way anywhere
    /// </summary>
    public class MqttMessageHelper
    {
        public static string GetSensorDeviceTypeTopic(string sensorDeviceTypeId)
        {
            return $"{Constant.TopicSensorDeviceType}/{sensorDeviceTypeId}";
        }
        public static string GetSensorDeviceTopic(string sensorDeviceId)
        {
            return $"{Constant.TopicSensorDevice}/{sensorDeviceId}";
        }
        public static string SerializePayload(object payload)
        {
            return JsonConvert.SerializeObject(payload);
        }
    }
}
