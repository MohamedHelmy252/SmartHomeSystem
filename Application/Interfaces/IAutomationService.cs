using Application.DTOs.Automation;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IAutomationService
    {
        Task<List<AutomationRule>> GetForHomeOwnerAsync(int userId);

        Task<AutomationRule?> GetByIdForHomeOwnerAsync(int userId, int ruleId);

        Task<AutomationRule> CreateAsync(int userId, CreateAutomationRuleDTO request);

        Task<bool> UpdateAsync(int userId, int ruleId, UpdateAutomationRuleDTO request);

        Task<bool> DeleteAsync(int userId, int ruleId);

        Task EvaluateSensorRulesAsync(int sensorId, decimal value);

        Task ExecuteScheduledRulesAsync();
    }
}
