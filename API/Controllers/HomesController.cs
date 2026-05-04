using Application.DTOs.Home;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomesController : Controller
    {
        private readonly IHomeService _homeService;

        public HomesController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllHomes()
        {
            var homes = await _homeService.GetAllHomesAsync();

            return Ok(new
            {
                success = true,
                data = homes.Select(h => new
                {
                    h.HomeId,
                    h.HomeName,
                    h.Address,
                    h.City,
                    h.Country,
                    h.OwnerUserId,
                    h.CreatedAt,
                    h.UpdatedAt
                })
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHomeById(int id)
        {
            var home = await _homeService.GetHomeByIdAsync(id);

            if (home == null)
                return NotFound(new
                {
                    success = false,
                    message = "Home not found"
                });

            return Ok(new
            {
                success = true,
                data = home
            });
        }

        [HttpGet("my-home")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> GetMyHome()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValue))
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid token"
                });

            var userId = int.Parse(userIdValue);

            var home = await _homeService.GetHomeByOwnerIdAsync(userId);

            if (home == null)
                return NotFound(new
                {
                    success = false,
                    message = "No home assigned to this user"
                });

            return Ok(new
            {
                success = true,
                data = home
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateHome(CreateHomeDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var home = await _homeService.CreateHomeAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Home created successfully",
                    data = home
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

        [HttpPut("{id}")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHome(int id, UpdateHomeDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _homeService.UpdateHomeAsync(id, request);

            if (!updated)
                return NotFound(new
                {
                    success = false,
                    message = "Home not found"
                });

            return Ok(new
            {
                success = true,
                message = "Home updated successfully"
            });
        }

        [HttpDelete("{id}")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHome(int id)
        {
            var deleted = await _homeService.DeleteHomeAsync(id);

            if (!deleted)
                return NotFound(new
                {
                    success = false,
                    message = "Home not found"
                });

            return Ok(new
            {
                success = true,
                message = "Home deleted successfully"
            });
        }
    }
}

