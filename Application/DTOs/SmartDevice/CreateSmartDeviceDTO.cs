using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.SmartDevice
{
    public class CreateSmartDeviceDTO
    {
        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string DeviceType { get; set; }

        public string CurrentState { get; set; } = "OFF";

        [Required]
        public string MQTTTopic { get; set; }

        public string MQTTStatusTopic { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public int ESP32DeviceId { get; set; }
    }
}
