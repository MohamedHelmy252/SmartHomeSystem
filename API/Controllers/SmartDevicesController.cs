using Application.DTOs.SmartDevice;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartDevicesController : ControllerBase
    {
        private readonly ISmartDeviceService _smartDeviceService;

        public SmartDevicesController(ISmartDeviceService smartDeviceService)
        {
            _smartDeviceService = smartDeviceService;
        }

        //  Admin API
        [HttpGet]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var devices = await _smartDeviceService.GetAllAsync();

            if (!devices.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No smart devices available",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = devices.Select(d => new
                {
                    d.SmartDeviceId,
                    d.DeviceName,
                    d.DeviceType,
                    d.CurrentState,
                    d.MQTTTopic,
                    d.MQTTStatusTopic,
                    d.IsActive,
                    d.ESP32DeviceId,
                    d.CreatedAt
                })
            });
        }

        //  Admin API
        [HttpGet("{id}")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var device = await _smartDeviceService.GetByIdAsync(id);

            if (device == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Smart device not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    device.SmartDeviceId,
                    device.DeviceName,
                    device.DeviceType,
                    device.CurrentState,
                    device.MQTTTopic,
                    device.MQTTStatusTopic,
                    device.IsActive,
                    device.ESP32DeviceId,
                    device.CreatedAt
                }
            });
        }

        //  Admin API
        [HttpGet("esp32/{esp32Id}")]
         [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByESP32Id(int esp32Id)
        {
            var devices = await _smartDeviceService.GetByESP32IdAsync(esp32Id);

            if (!devices.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No smart devices available for this ESP32",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = devices
            });
        }

        //  HomeOwner API
        [HttpGet("home-owner")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> GetMySmartDevices()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValue))
                return Unauthorized(new { success = false, message = "Invalid token" });

            var userId = int.Parse(userIdValue);

            var devices = await _smartDeviceService.GetForHomeOwnerAsync(userId);

            if (!devices.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No smart devices available",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = devices.Select(d => new
                {
                    d.SmartDeviceId,
                    d.DeviceName,
                    d.DeviceType,
                    d.CurrentState,
                    d.IsActive,
                    d.ESP32DeviceId,
                    RoomId = d.ESP32Device.RoomId,
                    RoomName = d.ESP32Device.Room.Name
                })
            });
        }

        //  HomeOwner API
        [HttpGet("home-owner/room/{roomId}")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> GetMySmartDevicesByRoom(int roomId)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValue))
                return Unauthorized(new { success = false, message = "Invalid token" });

            var userId = int.Parse(userIdValue);

            var devices = await _smartDeviceService.GetForHomeOwnerRoomAsync(userId, roomId);

            if (!devices.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No smart devices available in this room",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = devices.Select(d => new
                {
                    d.SmartDeviceId,
                    d.DeviceName,
                    d.DeviceType,
                    d.CurrentState,
                    d.IsActive,
                    d.ESP32DeviceId,
                    RoomId = d.ESP32Device.RoomId
                })
            });
        }

        //  Admin API
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateSmartDeviceDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var device = await _smartDeviceService.CreateAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Smart device created successfully",
                    data = device
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
        public async Task<IActionResult> Update(int id, UpdateSmartDeviceDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _smartDeviceService.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Smart device not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Smart device updated successfully"
            });
        }

        //  Admin API
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _smartDeviceService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Smart device not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Smart device deleted successfully"
            });
        }

        //  HomeOwner API
        [HttpPost("{id}/control")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> ControlDevice(int id, ControlSmartDeviceDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValue))
                return Unauthorized(new { success = false, message = "Invalid token" });

            var userId = int.Parse(userIdValue);

            try
            {
                var device = await _smartDeviceService.ControlAsync(userId, id, request.State);

                if (device == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Smart device not found or not assigned to your home"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"Device turned {device.CurrentState}",
                    data = new
                    {
                        device.SmartDeviceId,
                        device.DeviceName,
                        device.CurrentState
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
    }
}
