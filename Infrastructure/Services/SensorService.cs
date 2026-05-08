using Application.DTOs.Sensor;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SensorService : ISensorService
    {
        private readonly AppDbContext _context;

        public SensorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sensor>> GetAllAsync()
        {
            return await _context.Sensors
                .Include(s => s.ESP32Device)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Sensor?> GetByIdAsync(int id)
        {
            return await _context.Sensors
                .Include(s => s.ESP32Device)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SensorId == id);
        }

        public async Task<List<Sensor>> GetByESP32IdAsync(int esp32Id)
        {
            return await _context.Sensors
                .Where(s => s.ESP32DeviceId == esp32Id)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Sensor>> GetForHomeOwnerAsync(int userId)
        {
            var home = await _context.Homes
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.OwnerUserId == userId);

            if (home == null)
                return new List<Sensor>();

            return await _context.Sensors
                .Include(s => s.ESP32Device)
                .ThenInclude(e => e.Room)
                .Where(s => s.ESP32Device.Room.HomeId == home.HomeId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Sensor>> GetForHomeOwnerRoomAsync(int userId, int roomId)
        {
            var home = await _context.Homes
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.OwnerUserId == userId);

            if (home == null)
                return new List<Sensor>();

            var roomBelongsToOwner = await _context.Rooms
                .AnyAsync(r => r.RoomId == roomId && r.HomeId == home.HomeId);

            if (!roomBelongsToOwner)
                return new List<Sensor>();

            return await _context.Sensors
                .Include(s => s.ESP32Device)
                .Where(s => s.ESP32Device.RoomId == roomId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Sensor> CreateAsync(CreateSensorDTO request)
        {
            var espExists = await _context.ESP32Devices
                .AnyAsync(e => e.ESP32DeviceId == request.ESP32DeviceId);

            if (!espExists)
                throw new Exception("ESP32 device not found");

            var sensor = new Sensor
            {
                SensorName = request.SensorName,
                SensorType = request.SensorType,
                Unit = request.Unit,
                ThresholdValue = request.ThresholdValue,
                MQTTTopic = request.MQTTTopic,
                ESP32DeviceId = request.ESP32DeviceId,
                CreatedAt = DateTime.Now
            };

            _context.Sensors.Add(sensor);
            await _context.SaveChangesAsync();

            return sensor;
        }

        public async Task<bool> UpdateAsync(int id, UpdateSensorDTO request)
        {
            var sensor = await _context.Sensors
                .FirstOrDefaultAsync(s => s.SensorId == id);

            if (sensor == null)
                return false;

            sensor.SensorName = request.SensorName;
            sensor.SensorType = request.SensorType;
            sensor.Unit = request.Unit;
            sensor.ThresholdValue = request.ThresholdValue;
            sensor.MQTTTopic = request.MQTTTopic;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sensor = await _context.Sensors
                .FirstOrDefaultAsync(s => s.SensorId == id);

            if (sensor == null)
                return false;

            _context.Sensors.Remove(sensor);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
