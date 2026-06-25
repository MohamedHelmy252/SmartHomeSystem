using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class AutomationRule
    {
        public int RuleId { get; set; }

        public string RuleName { get; set; }

        public string? Description { get; set; }

        // SensorCondition / Schedule
        public string RuleType { get; set; }
        public string? ScheduleTriggerType { get; set; } // Start / End
        public string? ScheduleGroupId { get; set; }
        public int? SensorId { get; set; }

        public string? ConditionType { get; set; }

        public string? Operator { get; set; }

        public decimal? CompareValue { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        // Example: Sunday,Monday,Tuesday OR Daily
        public string? DaysOfWeek { get; set; }

        public bool IsActive { get; set; } = true;

        public int HomeId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Home Home { get; set; }

        public User User { get; set; }

        public Sensor? Sensor { get; set; }

        public ICollection<AutomationAction> Actions { get; set; } = new List<AutomationAction>();

        public ICollection<AutomationExecution> Executions { get; set; } = new List<AutomationExecution>();
    }
}