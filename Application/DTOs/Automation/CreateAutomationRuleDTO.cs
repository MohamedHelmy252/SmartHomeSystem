using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Automation
{
    public class CreateAutomationRuleDTO
    {
        public string RuleName { get; set; }

        public string? Description { get; set; }

        public string RuleType { get; set; } // SensorCondition / Schedule

        public int HomeId { get; set; }

        public int? SensorId { get; set; }

        public string? ConditionType { get; set; }

        public string? Operator { get; set; } // > < >= <= ==

        public decimal? CompareValue { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string? DaysOfWeek { get; set; } // Daily / Sunday,Monday

        public List<CreateAutomationActionDTO> Actions { get; set; }
            = new List<CreateAutomationActionDTO>();
    }
}
