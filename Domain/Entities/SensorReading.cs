using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class SensorReading
    {
        public int SensorReadingId { get; set; }

        public int SensorId { get; set; }

        public decimal Value { get; set; }

        public string Unit { get; set; }

        public DateTime ReadAt { get; set; }

        public Sensor Sensor { get; set; }
    }
}
