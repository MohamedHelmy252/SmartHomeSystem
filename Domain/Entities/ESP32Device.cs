using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ESP32Device
    {
        public int ESP32DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string MacAddress { get; set; }

        public string IpAddress { get; set; }

        public string FirmwareVersion { get; set; }

        public string ConnectionStatus { get; set; }

        public int RoomId { get; set; }

        public DateTime CreatedAt { get; set; }
        #region Relation 
        public Room Room { get; set; }

        public ICollection<SmartDevice> SmartDevices { get; set; }
        public ICollection<Sensor> Sensors { get; set; }
        #endregion
    }
}