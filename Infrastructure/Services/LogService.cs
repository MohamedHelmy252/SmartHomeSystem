using Application.DTOs.Logs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Services
{

    public class LogService : ILogService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogService(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<LogResponseDTO>> GetAllAsync()
        {
            return await _context.Logs
                  .OrderByDescending(l => l.CreatedAt)
                  .Select(l => new LogResponseDTO
                  {
                      LogId = l.LogId,
                      UserId = l.UserId,
                      HomeId = l.HomeId,
                      EventType = l.EventType,
                      Severity = l.Severity,
                      RiskScore = l.RiskScore,
                      Description = l.Description,
                      EntityName = l.EntityName,
                      EntityId = l.EntityId,
                      IpAddress = l.IpAddress,
                      UserAgent = l.UserAgent,
                      Endpoint = l.Endpoint,
                      HttpMethod = l.HttpMethod,
                      StatusCode = l.StatusCode,
                      ActorRole = l.ActorRole,
                      CreatedAt = l.CreatedAt
                  })
                  .ToListAsync();
        }

        public async Task<List<LogResponseDTO>> GetBySeverityAsync(string severity)
        {
            severity = severity.Trim();

            return await _context.Logs
                .Where(l => l.Severity == severity)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new LogResponseDTO
                {
                    LogId = l.LogId,
                    UserId = l.UserId,
                    HomeId = l.HomeId,
                    EventType = l.EventType,
                    Severity = l.Severity,
                    RiskScore = l.RiskScore,
                    Description = l.Description,
                    EntityName = l.EntityName,
                    EntityId = l.EntityId,
                    IpAddress = l.IpAddress,
                    UserAgent = l.UserAgent,
                    Endpoint = l.Endpoint,
                    HttpMethod = l.HttpMethod,
                    StatusCode = l.StatusCode,
                    ActorRole = l.ActorRole,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<LogResponseDTO>> GetCriticalAsync()
        {
            return await _context.Logs
                .Where(l => l.Severity == "Critical" || l.RiskScore >= 20)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new LogResponseDTO
                {
                    LogId = l.LogId,
                    UserId = l.UserId,
                    HomeId = l.HomeId,
                    EventType = l.EventType,
                    Severity = l.Severity,
                    RiskScore = l.RiskScore,
                    Description = l.Description,
                    EntityName = l.EntityName,
                    EntityId = l.EntityId,
                    IpAddress = l.IpAddress,
                    UserAgent = l.UserAgent,
                    Endpoint = l.Endpoint,
                    HttpMethod = l.HttpMethod,
                    StatusCode = l.StatusCode,
                    ActorRole = l.ActorRole,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();
        }

        public async Task LogAsync(
          string eventType,
          string severity,
          int riskScore,
          string description,
          string actorRole,
          int? userId = null,
          int? homeId = null,
          string? entityName = null,
          int? entityId = null,
          int? statusCode = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var remoteIp = httpContext?.Connection.RemoteIpAddress;

            string? ipAddress = null;

            if (remoteIp != null)
            {
                ipAddress = remoteIp.MapToIPv4().ToString();

                // لو شغال Local وهي ::1
                if (remoteIp.ToString() == "::1")
                {
                    ipAddress = "127.0.0.1";
                }
            }

            var userAgent = httpContext?.Request.Headers["User-Agent"].ToString();
            var endpoint = httpContext?.Request.Path.ToString();
            var httpMethod = httpContext?.Request.Method;

            var log = new Log
            {
                EventType = eventType,
                Severity = severity,
                RiskScore = riskScore,
                Description = description,
                ActorRole = actorRole,

                UserId = userId,
                HomeId = homeId,
                EntityName = entityName,
                EntityId = entityId,

                IpAddress = ipAddress,
                UserAgent = userAgent,
                Endpoint = endpoint,
                HttpMethod = httpMethod,
                StatusCode = statusCode,

                CreatedAt = DateTime.Now
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }







    }
}
