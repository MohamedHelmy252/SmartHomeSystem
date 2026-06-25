using System;

namespace Domain.Entities
{
    public class AutomationAction
    {
        public int ActionId { get; set; }

        public int RuleId { get; set; }

        // DeviceControl / Notification
        public string ActionType { get; set; }

        public int? TargetDeviceId { get; set; }

        // ON / OFF or notification value
        public string? ActionValue { get; set; }

        public string? MessageTemplate { get; set; }

        public int ExecutionOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public AutomationRule Rule { get; set; }

        public SmartDevice? TargetDevice { get; set; }
    }
}