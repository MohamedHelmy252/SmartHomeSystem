using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Log
    {
        public int LogId { get; set; }

        public string ActionType { get; set; }

        public string EntityName { get; set; }

        public int EntityId { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string Description { get; set; }

        public string UserAgent { get; set; }

        public int UserId { get; set; }

        public int HomeId { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        #region Relation 
        public User User { get; set; }

        public Home Home { get; set; }
        #endregion
    }
}