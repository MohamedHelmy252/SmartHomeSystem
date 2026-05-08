using Application.DTOs.SmartDevice;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SmartDeviceService : ISmartDeviceService
    {
        private readonly AppDbContext _context;

        public SmartDeviceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SmartDevice>> GetAllAsync()
        {
            return await _context.SmartDevices
                .Include(d => d.ESP32Device)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SmartDevice?> GetByIdAsync(int id)
        {
            return await _context.SmartDevices
                .Include(d => d.ESP32Device)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.SmartDeviceId == id);
        }

        public async Task<List<SmartDevice>> GetByESP32IdAsync(int esp32Id)
        {
            return await _context.SmartDevices
                .Where(d => d.ESP32DeviceId == esp32Id)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SmartDevice>> GetForHomeOwnerAsync(int userId)
        {
            var home = await _context.Homes
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.OwnerUserId == userId);

            if (home == null)
                return new List<SmartDevice>();

            return await _context.SmartDevices
                .Include(d => d.ESP32Device)
                .ThenInclude(e => e.Room)
                .Where(d => d.ESP32Device.Room.HomeId == home.HomeId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<SmartDevice>> GetForHomeOwnerRoomAsync(int userId, int roomId)
        {
            var home = await _context.Homes
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.OwnerUserId == userId);

            if (home == null)
                return new List<SmartDevice>();

            var roomBelongsToOwner = await _context.Rooms
                .AnyAsync(r => r.RoomId == roomId && r.HomeId == home.HomeId);

            if (!roomBelongsToOwner)
                return new List<SmartDevice>();

            return await _context.SmartDevices
                .Include(d => d.ESP32Device)
                .Where(d => d.ESP32Device.RoomId == roomId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SmartDevice> CreateAsync(CreateSmartDeviceDTO request)
        {
            var espExists = await _context.ESP32Devices
                .AnyAsync(e => e.ESP32DeviceId == request.ESP32DeviceId);

            if (!espExists)
                throw new Exception("ESP32 device not found");

            var device = new SmartDevice
            {
                DeviceName = request.DeviceName,
                DeviceType = request.DeviceType,
                CurrentState = request.CurrentState,
                MQTTTopic = request.MQTTTopic,
                MQTTStatusTopic = request.MQTTStatusTopic,
                IsActive = request.IsActive,
                ESP32DeviceId = request.ESP32DeviceId,
                CreatedAt = DateTime.Now
            };

            _context.SmartDevices.Add(device);
            await _context.SaveChangesAsync();

            return device;
        }

        public async Task<bool> UpdateAsync(int id, UpdateSmartDeviceDTO request)
        {
            var device = await _context.SmartDevices
                .FirstOrDefaultAsync(d => d.SmartDeviceId == id);

            if (device == null)
                return false;

            device.DeviceName = request.DeviceName;
            device.DeviceType = request.DeviceType;
            device.MQTTTopic = request.MQTTTopic;
            device.MQTTStatusTopic = request.MQTTStatusTopic;
            device.IsActive = request.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var device = await _context.SmartDevices
                .FirstOrDefaultAsync(d => d.SmartDeviceId == id);

            if (device == null)
                return false;

            _context.SmartDevices.Remove(device);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<SmartDevice?> ControlAsync(int userId, int deviceId, string state)
        {
            state = state.Trim().ToUpper();

            if (state != "ON" && state != "OFF")
                throw new Exception("Invalid state. Allowed values: ON, OFF");

            var home = await _context.Homes
                .FirstOrDefaultAsync(h => h.OwnerUserId == userId);

            if (home == null)
                return null;

            var device = await _context.SmartDevices
                .Include(d => d.ESP32Device)
                .ThenInclude(e => e.Room)
                .FirstOrDefaultAsync(d =>
                    d.SmartDeviceId == deviceId &&
                    d.ESP32Device.Room.HomeId == home.HomeId);

            if (device == null)
                return null;

            device.CurrentState = state;

            // TODO: هنا بعدين هنضيف MQTT Publish
            // await _mqttService.PublishAsync(device.MQTTTopic, state);

            await _context.SaveChangesAsync();

            return device;
        }
    }
}
