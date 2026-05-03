using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Alert
    {
        public int AlertId { get; set; }

        public string AlertType { get; set; }

        public string Severity { get; set; }

        public string Message { get; set; }

        public string Source { get; set; }

        public int HomeId { get; set; }

        public DateTime CreatedAt { get; set; }
        #region Relation 
        public Home Home { get; set; }

        public ICollection<Notification> Notifications { get; set; }
        #endregion
    }
}