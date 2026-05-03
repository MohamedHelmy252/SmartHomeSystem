using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class SmartDevice
    {
        public int SmartDeviceId { get; set; }

        public string DeviceName { get; set; }

        public string DeviceType { get; set; }

        public string CurrentState { get; set; }

        public string MQTTTopic { get; set; }

        public string MQTTStatusTopic { get; set; }

        public bool IsActive { get; set; }

        public int ESP32DeviceId { get; set; }

        public DateTime CreatedAt { get; set; }
        #region Relation 

        public ESP32Device ESP32Device { get; set; }
        #endregion
    }
}