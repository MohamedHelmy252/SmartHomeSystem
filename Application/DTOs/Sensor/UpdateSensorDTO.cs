using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Sensor
{

    public class UpdateSensorDTO
    {
        [Required]
        public string SensorName { get; set; }

        [Required]
        public string SensorType { get; set; }

        public string Unit { get; set; }

        public decimal ThresholdValue { get; set; }

        public string MQTTTopic { get; set; }
    }
}
