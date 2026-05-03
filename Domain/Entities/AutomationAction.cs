using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class AutomationAction
    {
        public int ActionId { get; set; }

        public int RuleId { get; set; }

        public string ActionType { get; set; }

        public int? TargetDeviceId { get; set; }

        public string ActionValue { get; set; }

        public string MessageTemplate { get; set; }

        public int ExecutionOrder { get; set; }

        public DateTime CreatedAt { get; set; }
        #region 
        public AutomationRule Rule { get; set; }

        public SmartDevice TargetDevice { get; set; }
        #endregion
    }
}