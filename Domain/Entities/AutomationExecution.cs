using System;

namespace Domain.Entities
{
    public class AutomationExecution
    {
        public int ExecutionId { get; set; }

        public int? RuleId { get; set; }

        public string? TriggeredValue { get; set; }

        public DateTime TriggeredAt { get; set; } = DateTime.Now;

        // Success / Failed / Skipped
        public string Status { get; set; }

        public AutomationRule? Rule { get; set; }
    }
}