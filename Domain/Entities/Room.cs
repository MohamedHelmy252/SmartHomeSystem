using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Room
    {
        public int RoomId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int HomeId { get; set; }

        public DateTime CreatedAt { get; set; }

        #region Relation 
        public Home Home { get; set; }
        public ICollection<ESP32Device> ESP32Devices { get; set; }
        #endregion
    }
}