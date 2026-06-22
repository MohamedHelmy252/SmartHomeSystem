using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.MQTT
{
    public class MqttSensorReadingMessage
    {
        public int SensorId { get; set; }

        public decimal Value { get; set; }

        public string Unit { get; set; }
    }
}
