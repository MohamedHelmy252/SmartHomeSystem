using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Home
    {
        public int HomeId { get; set; }

        public string HomeName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public int OwnerUserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        #region Relation 
        public User Owner { get; set; }

        public ICollection<Room> Rooms { get; set; }
        public ICollection<Camera> Cameras { get; set; }
        public ICollection<KnownFace> KnownFaces { get; set; }
        public ICollection<Alert> Alerts { get; set; }
        public ICollection<Log> Logs { get; set; }
        public ICollection<AutomationRule> AutomationRules { get; set; }
        #endregion
    }
}