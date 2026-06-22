using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.SmartDevice
{
    public class MqttDeviceStatusMessage
    {
        public int DeviceId { get; set; }

        public string State { get; set; }
    }
}
