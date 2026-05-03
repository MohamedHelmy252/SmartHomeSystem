using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Sensor
    {
        public int SensorId { get; set; }

        public string SensorName { get; set; }

        public string SensorType { get; set; }

        public string Unit { get; set; }

        public decimal ThresholdValue { get; set; }

        public string MQTTTopic { get; set; }

        public int ESP32DeviceId { get; set; }

        public DateTime CreatedAt { get; set; }

        #region Relation 
        public ESP32Device ESP32Device { get; set; }

        public ICollection<AutomationRule> AutomationRules { get; set; }
        #endregion
    }
}