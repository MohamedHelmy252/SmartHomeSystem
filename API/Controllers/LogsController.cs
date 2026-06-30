using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs()
        {
            var logs = await _logService.GetAllAsync();

            return Ok(new
            {
                success = true,
                count = logs.Count,
                data = logs
            });
        }

        [HttpGet("severity/{severity}")]
        public async Task<IActionResult> GetLogsBySeverity(string severity)
        {
            var logs = await _logService.GetBySeverityAsync(severity);

            return Ok(new
            {
                success = true,
                count = logs.Count,
                data = logs
            });
        }

        [HttpGet("critical")]
        public async Task<IActionResult> GetCriticalLogs()
        {
            var logs = await _logService.GetCriticalAsync();

            return Ok(new
            {
                success = true,
                count = logs.Count,
                data = logs
            });
        }
    }
}
