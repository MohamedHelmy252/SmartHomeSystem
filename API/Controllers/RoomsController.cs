using Application.DTOs.Room;
using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IHomeService _homeService;

        public RoomsController(IRoomService roomService, IHomeService homeService)
        {
            _roomService = roomService;
            _homeService = homeService;
        }

        // HomeOwner API
        [Authorize(Roles = "HomeOwner")]
        [HttpGet("home-owner")]
        public async Task<IActionResult> GetMyRooms()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var home = await _homeService.GetHomeByOwnerIdAsync(userId);

            if (home == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "No home found for this user"
                });
            }

            var rooms = await _roomService.GetRoomsByHomeIdAsync(home.HomeId);

            if (!rooms.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No rooms available",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = rooms
            });
        }
    

        //  Admin
        [Authorize(Roles = "Admin")]
        [HttpGet("home/{homeId}")]
        public async Task<IActionResult> GetRoomsByHome(int homeId)
        {
            var rooms = await _roomService.GetRoomsByHomeIdAsync(homeId);

            return Ok(new { success = true, data = rooms });
        }

        // Admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRoom(CreateRoomDTO request)
        {
            var room = await _roomService.CreateRoomAsync(request);

            return Ok(new
            {
                success = true,
                message = "Room created",
                data = room
            });
        }

        //  Admin
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, UpdateRoomDTO request)
        {
            var updated = await _roomService.UpdateRoomAsync(id, request);

            if (!updated)
                return NotFound(new { success = false, message = "Room not found" });

            return Ok(new { success = true, message = "Room updated" });
        }

        // Admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var deleted = await _roomService.DeleteRoomAsync(id);

            if (!deleted)
                return NotFound(new { success = false, message = "Room not found" });

            return Ok(new { success = true, message = "Room deleted" });
        }
    }


}

