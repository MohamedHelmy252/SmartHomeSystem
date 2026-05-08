using Application.DTOs.Sensor;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ISensorService _sensorService;

        public SensorsController(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        //  Admin API
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var sensors = await _sensorService.GetAllAsync();

            if (!sensors.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No sensors available",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = sensors.Select(s => new
                {
                    s.SensorId,
                    s.SensorName,
                    s.SensorType,
                    s.Unit,
                    s.ThresholdValue,
                    s.MQTTTopic,
                    s.ESP32DeviceId,
                    s.CreatedAt
                })
            });
        }

        //  Admin API
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var sensor = await _sensorService.GetByIdAsync(id);

            if (sensor == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Sensor not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    sensor.SensorId,
                    sensor.SensorName,
                    sensor.SensorType,
                    sensor.Unit,
                    sensor.ThresholdValue,
                    sensor.MQTTTopic,
                    sensor.ESP32DeviceId,
                    sensor.CreatedAt
                }
            });
        }

        //  Admin API
        [HttpGet("esp32/{esp32Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByESP32Id(int esp32Id)
        {
            var sensors = await _sensorService.GetByESP32IdAsync(esp32Id);

            if (!sensors.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No sensors available for this ESP32",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = sensors
            });
        }

        //  HomeOwner API
        [HttpGet("home-owner")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> GetMySensors()
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValue))
                return Unauthorized(new { success = false, message = "Invalid token" });

            var userId = int.Parse(userIdValue);

            var sensors = await _sensorService.GetForHomeOwnerAsync(userId);

            if (!sensors.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No sensors available",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = sensors.Select(s => new
                {
                    s.SensorId,
                    s.SensorName,
                    s.SensorType,
                    s.Unit,
                    s.ThresholdValue,
                    s.MQTTTopic,
                    s.ESP32DeviceId,
                    RoomId = s.ESP32Device.RoomId,
                    RoomName = s.ESP32Device.Room.Name
                })
            });
        }

        //  HomeOwner API
        [HttpGet("home-owner/room/{roomId}")]
        [Authorize(Roles = "HomeOwner")]
        public async Task<IActionResult> GetMySensorsByRoom(int roomId)
        {
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValue))
                return Unauthorized(new { success = false, message = "Invalid token" });

            var userId = int.Parse(userIdValue);

            var sensors = await _sensorService.GetForHomeOwnerRoomAsync(userId, roomId);

            if (!sensors.Any())
            {
                return Ok(new
                {
                    success = true,
                    message = "No sensors available in this room",
                    data = new List<object>()
                });
            }

            return Ok(new
            {
                success = true,
                data = sensors.Select(s => new
                {
                    s.SensorId,
                    s.SensorName,
                    s.SensorType,
                    s.Unit,
                    s.ThresholdValue,
                    s.MQTTTopic,
                    s.ESP32DeviceId,
                    RoomId = s.ESP32Device.RoomId
                })
            });
        }

        //  Admin API
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateSensorDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var sensor = await _sensorService.CreateAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Sensor created successfully",
                    data = sensor
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
        public async Task<IActionResult> Update(int id, UpdateSensorDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _sensorService.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Sensor not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Sensor updated successfully"
            });
        }

        //  Admin API
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _sensorService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Sensor not found"
                });
            }

            return Ok(new
            {
                success = true,
                message = "Sensor deleted successfully"
            });
        }
    }
}
