using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.ESP32Device
{
    public class UpdateESP32DeviceDTO
    {
        [Required]
        public string DeviceName { get; set; }

        public string IpAddress { get; set; }

        public string FirmwareVersion { get; set; }

        public string ConnectionStatus { get; set; }
    }
}
