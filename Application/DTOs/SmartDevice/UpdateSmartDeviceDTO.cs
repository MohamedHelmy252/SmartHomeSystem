using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.SmartDevice
{
    public class UpdateSmartDeviceDTO
    {
        [Required]
        public string DeviceName { get; set; }

        [Required]
        public string DeviceType { get; set; }

        public string MQTTTopic { get; set; }

        public string MQTTStatusTopic { get; set; }

        public bool IsActive { get; set; }
    }
}
