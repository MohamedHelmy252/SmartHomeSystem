using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Log
    {
        public int LogId { get; set; }

        public int? UserId { get; set; }
        public int? HomeId { get; set; }

        public string EventType { get; set; }
        public string Severity { get; set; }

        public string Description { get; set; }

        public string? EntityName { get; set; }
        public int? EntityId { get; set; }


        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public int RiskScore { get; set; }
        public string? Endpoint { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public string ActorRole { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }
        public Home? Home { get; set; }
    }
}