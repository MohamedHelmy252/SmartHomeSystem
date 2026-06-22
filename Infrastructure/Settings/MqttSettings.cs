using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Settings
{
    public class MqttSettings
    {
        public string Broker { get; set; }

        public int Port { get; set; }

        public string ClientId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool UseTls { get; set; }

        public string SensorReadingTopic { get; set; }

        public string DeviceControlTopic { get; set; }

        public string DeviceStatusTopic { get; set; }
    }
}
