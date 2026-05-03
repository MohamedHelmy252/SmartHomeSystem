using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class AutomationRule
    {
        public int RuleId { get; set; }

        public string RuleName { get; set; }

        public string Description { get; set; }

        public string RuleType { get; set; }

        public int SensorId { get; set; }

        public string ConditionType { get; set; }

        public string Operator { get; set; }

        public decimal CompareValue { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string DaysOfWeek { get; set; }

        public bool IsActive { get; set; }

        public int HomeId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }
        #region Relation 
        public Home Home { get; set; }

        public User User { get; set; }

        public Sensor Sensor { get; set; }

        public ICollection<AutomationAction> Actions { get; set; }

        public ICollection<AutomationExecution> Executions { get; set; }
        #endregion
    }
}