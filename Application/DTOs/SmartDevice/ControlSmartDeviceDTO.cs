using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.SmartDevice
{
    public class ControlSmartDeviceDTO
    {
        [Required]
        public string State { get; set; } // ON / OFF
    }
}
