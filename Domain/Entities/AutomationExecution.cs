using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class AutomationExecution
    {
        public int ExecutionId { get; set; }

        public int RuleId { get; set; }

        public string TriggeredValue { get; set; }

        public DateTime TriggeredAt { get; set; }

        public string Status { get; set; }
        #region Relation
        public AutomationRule Rule { get; set; }
        #endregion
    }
}