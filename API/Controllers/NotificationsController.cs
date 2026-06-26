using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "HomeOwner")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValue))
                return Unauthorized(new { success = false, message = "Invalid token" });

            var userId = int.Parse(userIdValue);

            var notifications = await _notificationService.GetUnreadAsync(userId);

            return Ok(new
            {
                success = true,
                count = notifications.Count,
                data = notifications.Select(n => new
                {
                    n.NotificationId,
                    n.Title,
                    n.Message,
                    n.IsRead,
                    n.CreatedAt
                })
            });
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValue))
                return Unauthorized(new { success = false, message = "Invalid token" });

            var userId = int.Parse(userIdValue);

            var updated = await _notificationService.MarkAsReadAsync(userId, id);

            if (!updated)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Notification not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Notification marked as read"
            });
        }
    }
}
