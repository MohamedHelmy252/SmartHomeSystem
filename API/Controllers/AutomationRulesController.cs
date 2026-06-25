using Application.DTOs.Automation;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{


    [Route("api/[controller]")]

    [ApiController]

    public class AutomationRulesController : ControllerBase

    {

        private readonly IAutomationService _automationService;



        public AutomationRulesController(IAutomationService automationService)

        {

            _automationService = automationService;

        }



        // HomeOwner API: Get all automation rules for logged-in user

        [HttpGet("home-owner")]

        [Authorize(Roles = "HomeOwner")]

        public async Task<IActionResult> GetMyAutomationRules()

        {

            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;



            if (string.IsNullOrEmpty(userIdValue))

                return Unauthorized(new { success = false, message = "Invalid token" });



            var userId = int.Parse(userIdValue);



            var rules = await _automationService.GetForHomeOwnerAsync(userId);



            if (rules == null || !rules.Any())

            {

                return Ok(new

                {

                    success = true,

                    message = "No automation rules available",

                    data = new List<object>()

                });

            }



            return Ok(new

            {

                success = true,

                data = rules.Select(r => new

                {

                    r.RuleId,

                    r.RuleName,

                    r.Description,

                    r.RuleType,

                    r.HomeId,

                    r.UserId,

                    r.SensorId,

                    SensorName = r.Sensor != null ? r.Sensor.SensorName : null,

                    r.ConditionType,

                    r.Operator,

                    r.CompareValue,

                    r.StartTime,

                    r.EndTime,

                    r.DaysOfWeek,

                    r.IsActive,

                    r.CreatedAt,

                    Actions = r.Actions.Select(a => new

                    {

                        a.ActionId,

                        a.ActionType,

                        a.TargetDeviceId,

                        TargetDeviceName = a.TargetDevice != null ? a.TargetDevice.DeviceName : null,

                        a.ActionValue,

                        a.MessageTemplate,

                        a.ExecutionOrder

                    }),

                    Executions = r.Executions

                        .OrderByDescending(e => e.TriggeredAt)

                        .Take(5)

                        .Select(e => new

                        {

                            e.ExecutionId,

                            e.TriggeredValue,

                            e.TriggeredAt,

                            e.Status

                        })

                })

            });

        }



        // HomeOwner API: Get automation rule by id

        [HttpGet("{id}")]

        [Authorize(Roles = "HomeOwner")]

        public async Task<IActionResult> GetById(int id)

        {

            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;



            if (string.IsNullOrEmpty(userIdValue))

                return Unauthorized(new { success = false, message = "Invalid token" });



            var userId = int.Parse(userIdValue);



            var rule = await _automationService.GetByIdForHomeOwnerAsync(userId, id);



            if (rule == null)

            {

                return NotFound(new

                {

                    success = false,

                    message = "Automation rule not found"

                });

            }



            return Ok(new

            {

                success = true,

                data = new

                {

                    rule.RuleId,

                    rule.RuleName,

                    rule.Description,

                    rule.RuleType,

                    rule.HomeId,

                    rule.UserId,

                    rule.SensorId,

                    SensorName = rule.Sensor != null ? rule.Sensor.SensorName : null,

                    rule.ConditionType,

                    rule.Operator,

                    rule.CompareValue,

                    rule.StartTime,

                    rule.EndTime,

                    rule.DaysOfWeek,

                    rule.IsActive,

                    rule.CreatedAt,

                    Actions = rule.Actions.Select(a => new

                    {

                        a.ActionId,

                        a.ActionType,

                        a.TargetDeviceId,

                        TargetDeviceName = a.TargetDevice != null ? a.TargetDevice.DeviceName : null,

                        a.ActionValue,

                        a.MessageTemplate,

                        a.ExecutionOrder

                    }),

                    Executions = rule.Executions

                        .OrderByDescending(e => e.TriggeredAt)

                        .Take(10)

                        .Select(e => new

                        {

                            e.ExecutionId,

                            e.TriggeredValue,

                            e.TriggeredAt,

                            e.Status

                        })

                }

            });

        }



        // HomeOwner API: Create automation rule

        [HttpPost]

        [Authorize(Roles = "HomeOwner")]

        public async Task<IActionResult> Create(CreateAutomationRuleDTO request)

        {

            if (!ModelState.IsValid)

                return BadRequest(ModelState);



            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;



            if (string.IsNullOrEmpty(userIdValue))

                return Unauthorized(new { success = false, message = "Invalid token" });



            var userId = int.Parse(userIdValue);



            try

            {

                var rule = await _automationService.CreateAsync(userId, request);



                return Ok(new

                {

                    success = true,

                    message = "Automation rule created successfully",

                    data = new

                    {

                        rule.RuleId,

                        rule.RuleName,

                        rule.RuleType,

                        rule.HomeId,

                        rule.UserId,

                        rule.IsActive,

                        rule.CreatedAt

                    }

                });

            }

            catch (Exception ex)

            {

                return BadRequest(new

                {

                    success = false,

                    message = ex.Message

                });

            }

        }



        // HomeOwner API: Update automation rule

        [HttpPut("{id}")]

        [Authorize(Roles = "HomeOwner")]

        public async Task<IActionResult> Update(int id, UpdateAutomationRuleDTO request)

        {

            if (!ModelState.IsValid)

                return BadRequest(ModelState);



            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;



            if (string.IsNullOrEmpty(userIdValue))

                return Unauthorized(new { success = false, message = "Invalid token" });



            var userId = int.Parse(userIdValue);



            var updated = await _automationService.UpdateAsync(userId, id, request);



            if (!updated)

            {

                return NotFound(new

                {

                    success = false,

                    message = "Automation rule not found"

                });

            }



            return Ok(new

            {

                success = true,

                message = "Automation rule updated successfully"

            });

        }



        // HomeOwner API: Delete automation rule

        [HttpDelete("{id}")]

        [Authorize(Roles = "HomeOwner")]

        public async Task<IActionResult> Delete(int id)

        {

            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;



            if (string.IsNullOrEmpty(userIdValue))

                return Unauthorized(new { success = false, message = "Invalid token" });



            var userId = int.Parse(userIdValue);



            var deleted = await _automationService.DeleteAsync(userId, id);



            if (!deleted)

            {

                return NotFound(new

                {

                    success = false,

                    message = "Automation rule not found"

                });

            }



            return Ok(new

            {

                success = true,

                message = "Automation rule deleted successfully"

            });

        }

    }
    
}
