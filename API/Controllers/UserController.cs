using Application.DTOs.User;
using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IuserService _userService;
        private readonly AppDbContext _appDbContext;
        private readonly ILogService _logService;

        public UserController(IuserService userService , AppDbContext appDbContext ,ILogService logService  )
        {
            _userService = userService;
            _appDbContext = appDbContext;
             _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            var result = users.Select(u => new
            {
                u.UserId,
                u.FullName,
                u.PhoneNumber,
                u.Email,
                u.Role,
                u.IsActive,
                u.CreatedAt,
                u.UpdatedAt
            });

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    user.UserId,
                    user.FullName,
                    user.PhoneNumber,
                    user.Email,
                    user.Role,
                    user.IsActive,
                    user.CreatedAt,
                    user.UpdatedAt
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "User deleted successfully"
            });
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, UpdateUserRoleDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var targetUser = await _appDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (targetUser == null)
            {
                await _logService.LogAsync(
                    eventType: "AdminChangedUserRoleFailed",
                    severity: "Warning",
                    riskScore: 6,
                    description: $"Admin tried to change role for non-existing user. UserId: {id}",
                    actorRole: "Admin",
                    userId: adminId,
                    entityName: "User",
                    entityId: id,
                    statusCode: 404
                );

                return NotFound(new
                {
                    success = false,
                    message = "User not found"
                });
            }

            var oldRole = targetUser.Role;

            try
            {
                var updated = await _userService.UpdateUserRoleAsync(id, request.Role);

                if (!updated)
                {
                    await _logService.LogAsync(
                        eventType: "AdminChangedUserRoleFailed",
                        severity: "Warning",
                        riskScore: 6,
                        description: $"Admin failed to change role for user {id}.",
                        actorRole: "Admin",
                        userId: adminId,
                        entityName: "User",
                        entityId: id,
                        statusCode: 404
                    );

                    return NotFound(new
                    {
                        success = false,
                        message = "User not found"
                    });
                }

                await _logService.LogAsync(
                    eventType: "AdminChangedUserRole",
                    severity: "Information",
                    riskScore: 4,
                    description: $"Admin changed user role from {oldRole} to {request.Role}.",
                    actorRole: "Admin",
                    userId: adminId,
                    entityName: "User",
                    entityId: id,
                    statusCode: 200
                );

                return Ok(new
                {
                    success = true,
                    message = "User role updated successfully"
                });
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(
                    eventType: "AdminChangedUserRoleFailed",
                    severity: "Warning",
                    riskScore: 6,
                    description: $"Admin role change failed for user {id}. Reason: {ex.Message}",
                    actorRole: "Admin",
                    userId: adminId,
                    entityName: "User",
                    entityId: id,
                    statusCode: 400
                );

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }






    }


}
