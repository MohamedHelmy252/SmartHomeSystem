using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Automation
{
    public class CreateAutomationActionDTO
    {
        public string ActionType { get; set; }   // DeviceControl / Notification
        public string? ScheduleTriggerType { get; set; }
        public int? TargetDeviceId { get; set; }
        public string? ActionValue { get; set; } // ON / OFF

        public string? MessageTemplate { get; set; }

        public int ExecutionOrder { get; set; }
    }
}
