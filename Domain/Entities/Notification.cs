using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string ChannelType { get; set; }

        public string DeliveryStatus { get; set; }

        public int AlertId { get; set; }

        public DateTime SentAt { get; set; }
        #region Relation 
        public Alert Alert { get; set; }
        #endregion
    }
}