using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Sensor
{
    public class CreateSensorDTO
    {
        [Required]
        public string SensorName { get; set; }

        [Required]
        public string SensorType { get; set; }

        public string Unit { get; set; }

        public decimal ThresholdValue { get; set; }

        [Required]
        public string MQTTTopic { get; set; }

        [Required]
        public int ESP32DeviceId { get; set; }
    }
}
