using Application.DTOs.Automation;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class AutomationService : IAutomationService
    {
        private readonly AppDbContext _context;
        private readonly IMqttService _mqttService;
        private readonly INotificationService _notificationService;
        public AutomationService(AppDbContext context,
            IMqttService mqttService,
            INotificationService notificationService)
        {
            _context = context;
            _mqttService = mqttService;
            _notificationService = notificationService;
        }

        public async Task<List<AutomationRule>> GetForHomeOwnerAsync(int userId)
        {
            return await _context.AutomationRules
                .Include(r => r.Sensor)
                .Include(r => r.Actions)
                .ThenInclude(a => a.TargetDevice)
                .Include(r => r.Executions)
                .Where(r =>
                    r.UserId == userId &&
                    (
                        r.RuleType != "Schedule" ||
                        r.ScheduleTriggerType == "Start"
                    ))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<AutomationRule?> GetByIdForHomeOwnerAsync(int userId, int ruleId)
        {
            var rule = await _context.AutomationRules
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RuleId == ruleId && r.UserId == userId);

            if (rule == null)
                return null;

            if (rule.RuleType == "Schedule" && !string.IsNullOrWhiteSpace(rule.ScheduleGroupId))
            {
                var startRule = await _context.AutomationRules
                    .Include(r => r.Sensor)
                    .Include(r => r.Actions)
                    .ThenInclude(a => a.TargetDevice)
                    .Include(r => r.Executions)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r =>
                        r.ScheduleGroupId == rule.ScheduleGroupId &&
                        r.ScheduleTriggerType == "Start" &&
                        r.UserId == userId);

                return startRule;
            }

            return await _context.AutomationRules
                .Include(r => r.Sensor)
                .Include(r => r.Actions)
                .ThenInclude(a => a.TargetDevice)
                .Include(r => r.Executions)
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.RuleId == ruleId &&
                    r.UserId == userId);
        }

        public async Task<AutomationRule> CreateAsync(int userId, CreateAutomationRuleDTO request)
        {
            request.RuleType = request.RuleType.Trim();

            if (request.RuleType != "SensorCondition" && request.RuleType != "Schedule")
                throw new Exception("Invalid RuleType. Allowed values: SensorCondition, Schedule");

            var home = await _context.Homes
                .FirstOrDefaultAsync(h =>
                    h.HomeId == request.HomeId &&
                    h.OwnerUserId == userId);

            if (home == null)
                throw new Exception("Home not found or not assigned to this user");

            if (request.RuleType == "SensorCondition")
            {
                if (request.SensorId == null)
                    throw new Exception("SensorId is required for SensorCondition rule");

                if (string.IsNullOrWhiteSpace(request.Operator))
                    throw new Exception("Operator is required for SensorCondition rule");

                if (request.CompareValue == null)
                    throw new Exception("CompareValue is required for SensorCondition rule");

                var sensorExists = await _context.Sensors
                    .Include(s => s.ESP32Device)
                    .ThenInclude(e => e.Room)
                    .AnyAsync(s =>
                        s.SensorId == request.SensorId &&
                        s.ESP32Device.Room.HomeId == request.HomeId);

                if (!sensorExists)
                    throw new Exception("Sensor not found in this home");
            }

            if (request.RuleType == "Schedule")
            {
                if (request.StartTime == null)
                    throw new Exception("StartTime is required for Schedule rule");

                if (request.EndTime != null && request.EndTime <= request.StartTime)
                    throw new Exception("EndTime must be greater than StartTime");
            }

            if (request.Actions == null || !request.Actions.Any())
                throw new Exception("At least one action is required");

            foreach (var action in request.Actions)
                ValidateAction(action, request.HomeId);

            if (request.RuleType == "Schedule" && request.EndTime != null)
            {
                var scheduleGroupId = Guid.NewGuid().ToString();

                var startRule = BuildScheduleRule(
                    request,
                    userId,
                    request.StartTime!.Value,
                    "Start",
                    request.Actions,
                    reverseActions: false,
                    scheduleGroupId);

                var endRule = BuildScheduleRule(
                    request,
                    userId,
                    request.EndTime.Value,
                    "End",
                    request.Actions,
                    reverseActions: true,
                    scheduleGroupId);

                _context.AutomationRules.Add(startRule);
                _context.AutomationRules.Add(endRule);

                await _context.SaveChangesAsync();

                return startRule;
            }

            var rule = new AutomationRule
            {
                RuleName = request.RuleName,
                Description = request.Description,
                RuleType = request.RuleType,
                ScheduleTriggerType = request.RuleType == "Schedule" ? "Start" : null,
                ScheduleGroupId = request.RuleType == "Schedule" ? Guid.NewGuid().ToString() : null,
                HomeId = request.HomeId,
                UserId = userId,
                SensorId = request.SensorId,
                ConditionType = request.ConditionType,
                Operator = request.Operator,
                CompareValue = request.CompareValue,
                StartTime = request.StartTime,
                EndTime = null,
                DaysOfWeek = request.DaysOfWeek,
                IsActive = true,
                CreatedAt = DateTime.Now,
                Actions = request.Actions
                    .OrderBy(a => a.ExecutionOrder)
                    .Select(a => new AutomationAction
                    {
                        ActionType = a.ActionType,
                        TargetDeviceId = a.TargetDeviceId,
                        ActionValue = a.ActionValue,
                        MessageTemplate = a.MessageTemplate,
                        ExecutionOrder = a.ExecutionOrder,
                        CreatedAt = DateTime.Now
                    })
                    .ToList()
            };

            _context.AutomationRules.Add(rule);
            await _context.SaveChangesAsync();

            return rule;
        }

        public async Task<bool> UpdateAsync(int userId, int ruleId, UpdateAutomationRuleDTO request)
        {
            var rule = await _context.AutomationRules
                .FirstOrDefaultAsync(r =>
                    r.RuleId == ruleId &&
                    r.UserId == userId);

            if (rule == null)
                return false;

            if (rule.RuleType == "Schedule" && !string.IsNullOrWhiteSpace(rule.ScheduleGroupId))
            {
                var groupRules = await _context.AutomationRules
                    .Where(r =>
                        r.ScheduleGroupId == rule.ScheduleGroupId &&
                        r.UserId == userId)
                    .ToListAsync();

                foreach (var item in groupRules)
                {
                    item.Description = request.Description;
                    item.IsActive = request.IsActive;
                    item.DaysOfWeek = request.DaysOfWeek;

                    if (item.ScheduleTriggerType == "Start")
                    {
                        item.RuleName = request.RuleName + " - Start";
                        item.StartTime = request.StartTime;
                    }

                    if (item.ScheduleTriggerType == "End")
                    {
                        item.RuleName = request.RuleName + " - End";
                        item.StartTime = request.EndTime;
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }

            rule.RuleName = request.RuleName;
            rule.Description = request.Description;
            rule.IsActive = request.IsActive;
            rule.StartTime = request.StartTime;
            rule.EndTime = request.EndTime;
            rule.DaysOfWeek = request.DaysOfWeek;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int userId, int ruleId)
        {
            var rule = await _context.AutomationRules
                .FirstOrDefaultAsync(r =>
                    r.RuleId == ruleId &&
                    r.UserId == userId);

            if (rule == null)
                return false;

            List<AutomationRule> rulesToDelete;

            if (rule.RuleType == "Schedule" && !string.IsNullOrWhiteSpace(rule.ScheduleGroupId))
            {
                rulesToDelete = await _context.AutomationRules
                    .Where(r =>
                        r.ScheduleGroupId == rule.ScheduleGroupId &&
                        r.UserId == userId)
                    .ToListAsync();
            }
            else
            {
                rulesToDelete = new List<AutomationRule> { rule };
            }

            var ruleIds = rulesToDelete.Select(r => r.RuleId).ToList();

            var actions = await _context.AutomationActions
                .Where(a => ruleIds.Contains(a.RuleId))
                .ToListAsync();

            _context.AutomationActions.RemoveRange(actions);
            _context.AutomationRules.RemoveRange(rulesToDelete);

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task EvaluateSensorRulesAsync(int sensorId, decimal value)
        {
            var rules = await _context.AutomationRules
                .Include(r => r.Actions)
                .ThenInclude(a => a.TargetDevice)
                .Where(r =>
                    r.IsActive &&
                    r.RuleType == "SensorCondition" &&
                    r.SensorId == sensorId)
                .ToListAsync();

            foreach (var rule in rules)
            {
                var matched = CheckCondition(value, rule.Operator, rule.CompareValue);

                if (!matched)
                    continue;

                await ExecuteRuleAsync(rule, value.ToString());
            }
        }

        public async Task ExecuteScheduledRulesAsync()
        {
            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;
            var currentDay = now.DayOfWeek.ToString();

            var rules = await _context.AutomationRules
                .Include(r => r.Actions)
                .ThenInclude(a => a.TargetDevice)
                .Where(r =>
                    r.IsActive &&
                    r.RuleType == "Schedule" &&
                    r.StartTime != null)
                .ToListAsync();

            foreach (var rule in rules)
            {
                if (!IsDayAllowed(rule.DaysOfWeek, currentDay))
                    continue;

                var startTime = rule.StartTime.Value;
                var windowEnd = startTime.Add(TimeSpan.FromMinutes(2));

                if (currentTime < startTime || currentTime > windowEnd)
                    continue;

                var executedToday = await _context.AutomationExecutions
                    .AnyAsync(e =>
                        e.RuleId == rule.RuleId &&
                        e.TriggeredAt.Date == now.Date &&
                        e.Status == "Success");

                if (executedToday)
                    continue;

                await ExecuteRuleAsync(
                    rule,
                    $"Schedule-{rule.ScheduleTriggerType ?? "Start"} {currentTime}");
            }
        }

        private bool CheckCondition(decimal value, string? op, decimal? compareValue)
        {
            if (string.IsNullOrWhiteSpace(op) || compareValue == null)
                return false;

            return op switch
            {
                ">" => value > compareValue,
                "<" => value < compareValue,
                ">=" => value >= compareValue,
                "<=" => value <= compareValue,
                "==" => value == compareValue,
                "!=" => value != compareValue,
                _ => false
            };
        }

        private bool IsDayAllowed(string? daysOfWeek, string currentDay)
        {
            if (string.IsNullOrWhiteSpace(daysOfWeek))
                return true;

            if (daysOfWeek == "Daily")
                return true;

            return daysOfWeek.Contains(currentDay);
        }

        private string GetOppositeState(string state)
        {
            state = state.Trim().ToUpper();
            return state == "ON" ? "OFF" : "ON";
        }

        private void ValidateAction(CreateAutomationActionDTO action, int homeId)
        {
            if (action.ActionType != "DeviceControl" && action.ActionType != "Notification")
                throw new Exception("Invalid ActionType. Allowed values: DeviceControl, Notification");

            if (action.ActionType == "DeviceControl")
            {
                if (action.TargetDeviceId == null)
                    throw new Exception("TargetDeviceId is required for DeviceControl action");

                if (string.IsNullOrWhiteSpace(action.ActionValue))
                    throw new Exception("ActionValue is required for DeviceControl action");

                action.ActionValue = action.ActionValue.Trim().ToUpper();

                if (action.ActionValue != "ON" && action.ActionValue != "OFF")
                    throw new Exception("ActionValue must be ON or OFF");

                var deviceExists = _context.SmartDevices
                    .Include(d => d.ESP32Device)
                    .ThenInclude(e => e.Room)
                    .Any(d =>
                        d.SmartDeviceId == action.TargetDeviceId &&
                        d.ESP32Device.Room.HomeId == homeId);

                if (!deviceExists)
                    throw new Exception("Target device not found in this home");
            }

            if (action.ActionType == "Notification")
            {
                if (string.IsNullOrWhiteSpace(action.MessageTemplate))
                    throw new Exception("MessageTemplate is required for Notification action");
            }
        }

        private AutomationRule BuildScheduleRule(
            CreateAutomationRuleDTO request,
            int userId,
            TimeSpan triggerTime,
            string triggerType,
            List<CreateAutomationActionDTO> actions,
            bool reverseActions,
            string scheduleGroupId)
        {
            return new AutomationRule
            {
                RuleName = $"{request.RuleName} - {triggerType}",
                Description = request.Description,
                RuleType = "Schedule",
                ScheduleTriggerType = triggerType,
                ScheduleGroupId = scheduleGroupId,
                HomeId = request.HomeId,
                UserId = userId,
                SensorId = null,
                ConditionType = null,
                Operator = null,
                CompareValue = null,
                StartTime = triggerTime,
                EndTime = null,
                DaysOfWeek = request.DaysOfWeek,
                IsActive = true,
                CreatedAt = DateTime.Now,
                Actions = actions
                    .OrderBy(a => a.ExecutionOrder)
                    .Select(a =>
                    {
                        var value = a.ActionValue;

                        if (a.ActionType == "DeviceControl" && reverseActions && value != null)
                            value = GetOppositeState(value);

                        return new AutomationAction
                        {
                            ActionType = a.ActionType,
                            TargetDeviceId = a.TargetDeviceId,
                            ActionValue = value,
                            MessageTemplate = a.MessageTemplate,
                            ExecutionOrder = a.ExecutionOrder,
                            CreatedAt = DateTime.Now
                        };
                    })
                    .ToList()
            };
        }

        private async Task ExecuteRuleAsync(AutomationRule rule, string triggeredValue)
        {
            try
            {
                foreach (var action in rule.Actions.OrderBy(a => a.ExecutionOrder))
                {
                    if (action.ActionType == "DeviceControl")
                    {
                        if (action.TargetDevice == null)
                            continue;

                        var state = action.ActionValue?.Trim().ToUpper();

                        if (state != "ON" && state != "OFF")
                            continue;

                        var topic = action.TargetDevice.MQTTTopic;

                        var payload = JsonSerializer.Serialize(new
                        {
                            deviceId = action.TargetDevice.SmartDeviceId,
                            state = state
                        });

                        await _mqttService.PublishAsync(topic, payload);

                        action.TargetDevice.CurrentState = "PENDING_" + state;
                    }

                    if (action.ActionType == "Notification")
                    {
                        Console.WriteLine($"Automation Notification: {action.MessageTemplate}");
                    }
                }

                _context.AutomationExecutions.Add(new AutomationExecution
                {
                    RuleId = rule.RuleId,
                    TriggeredValue = triggeredValue,
                    TriggeredAt = DateTime.Now,
                    Status = "Success"
                });

                await _notificationService.CreateAsync(
                 rule.UserId,
                  "Automation Executed",
                  $"Automation rule '{rule.RuleName}' executed successfully."
                    );

                await _context.SaveChangesAsync();
            }
            catch
            {
                _context.AutomationExecutions.Add(new AutomationExecution
                {
                    RuleId = rule.RuleId,
                    TriggeredValue = triggeredValue,
                    TriggeredAt = DateTime.Now,
                    Status = "Failed"
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}