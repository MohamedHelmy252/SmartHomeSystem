using Application.DTOs.Logs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ILogService
    {
        Task LogAsync(
            string eventType,
            string severity,
            int riskScore,
            string description,
            string actorRole,
            int? userId = null,
            int? homeId = null,
            string? entityName = null,
            int? entityId = null,
            int? statusCode = null
        );


        Task<List<LogResponseDTO>> GetAllAsync();
        Task<List<LogResponseDTO>> GetBySeverityAsync(string severity);
        Task<List<LogResponseDTO>> GetCriticalAsync();
    }
}
