using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Automation
{
    public class UpdateAutomationRuleDTO
    {
        public string RuleName { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string? DaysOfWeek { get; set; }
    }
}
