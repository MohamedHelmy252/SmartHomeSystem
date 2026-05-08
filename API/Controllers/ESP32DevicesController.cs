using Application.DTOs.ESP32Device;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ESP32DevicesController : Controller
    {
        private readonly IESP32DeviceService _esp32DeviceService;

        public ESP32DevicesController(IESP32DeviceService esp32DeviceService)
        {
            _esp32DeviceService = esp32DeviceService;
        }

        //  Admin API
        [HttpGet]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var devices = await _esp32DeviceService.GetAllAsync();

            if (!devices.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No ESP32 devices available",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = devices.Select(e => new
                {
                    e.ESP32DeviceId,
                    e.DeviceName,
                    e.MacAddress,
                    e.IpAddress,
                    e.FirmwareVersion,
                    e.ConnectionStatus,
                    e.RoomId,
                    e.CreatedAt
                })
            });
        }

        //  Admin API
        [HttpGet("{id}")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var device = await _esp32DeviceService.GetByIdAsync(id);

            if (device == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "ESP32 device not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    device.ESP32DeviceId,
                    device.DeviceName,
                    device.MacAddress,
                    device.IpAddress,
                    device.FirmwareVersion,
                    device.ConnectionStatus,
                    device.RoomId,
                    device.CreatedAt
                }
            });
        }

        //  Admin API
        [HttpGet("room/{roomId}")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByRoomId(int roomId)
        {
            var devices = await _esp32DeviceService.GetByRoomIdAsync(roomId);

            if (!devices.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No ESP32 devices available in this room",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = devices.Select(e => new
                {
                    e.ESP32DeviceId,
                    e.DeviceName,
                    e.MacAddress,
                    e.IpAddress,
                    e.FirmwareVersion,
                    e.ConnectionStatus,
                    e.RoomId,
                    e.CreatedAt
                })
            });
        }

        //  HomeOwner API
        [HttpGet("home-owner")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> GetMyESP32Devices()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValue))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid token"
                });
            }

            var userId = int.Parse(userIdValue);

            var devices = await _esp32DeviceService.GetForHomeOwnerAsync(userId);

            if (!devices.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No ESP32 devices available",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = devices.Select(e => new
                {
                    e.ESP32DeviceId,
                    e.DeviceName,
                    e.MacAddress,
                    e.IpAddress,
                    e.FirmwareVersion,
                    e.ConnectionStatus,
                    e.RoomId,
                    e.CreatedAt
                })
            });
        }

        //  HomeOwner API
        [HttpGet("home-owner/room/{roomId}")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> GetMyESP32DevicesByRoom(int roomId)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValue))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid token"
                });
            }

            var userId = int.Parse(userIdValue);

            var devices = await _esp32DeviceService.GetForHomeOwnerRoomAsync(userId, roomId);

            if (!devices.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No ESP32 devices available in this room",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = devices.Select(e => new
                {
                    e.ESP32DeviceId,
                    e.DeviceName,
                    e.MacAddress,
                    e.IpAddress,
                    e.FirmwareVersion,
                    e.ConnectionStatus,
                    e.RoomId,
                    e.CreatedAt
                })
            });
        }

        //  Admin API
        [HttpPost]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateESP32DeviceDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var device = await _esp32DeviceService.CreateAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "ESP32 device created successfully",
                    data = new
                    {
                        device.ESP32DeviceId,
                        device.DeviceName,
                        device.MacAddress,
                        device.IpAddress,
                        device.FirmwareVersion,
                        device.ConnectionStatus,
                        device.RoomId,
                        device.CreatedAt
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

        //  Admin API
        [HttpPut("{id}")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateESP32DeviceDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _esp32DeviceService.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound(new
                {
                    success = false,
                    message = "ESP32 device not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "ESP32 device updated successfully"
            });
        }

        //  Admin API
        [HttpDelete("{id}")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _esp32DeviceService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "ESP32 device not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "ESP32 device deleted successfully"
            });
        }
    }
}
