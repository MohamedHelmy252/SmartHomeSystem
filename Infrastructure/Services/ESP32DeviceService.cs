using Application.DTOs.ESP32Device;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class ESP32DeviceService : IESP32DeviceService
    {

        private readonly AppDbContext _context;

        public ESP32DeviceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ESP32Device>> GetAllAsync()
        {
            return await _context.ESP32Devices
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ESP32Device?> GetByIdAsync(int id)
        {
            return await _context.ESP32Devices
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ESP32DeviceId == id);
        }

        public async Task<List<ESP32Device>> GetByRoomIdAsync(int roomId)
        {
            return await _context.ESP32Devices
                .Where(e => e.RoomId == roomId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ESP32Device>> GetForHomeOwnerAsync(int userId)
        {
            var home = await _context.Homes
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.OwnerUserId == userId);

            if (home == null)
                return new List<ESP32Device>();

            return await _context.ESP32Devices
                .Where(e => e.Room.HomeId == home.HomeId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<ESP32Device>> GetForHomeOwnerRoomAsync(int userId, int roomId)
        {
            var home = await _context.Homes
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.OwnerUserId == userId);

            if (home == null)
                return new List<ESP32Device>();

            var roomBelongsToOwner = await _context.Rooms
                .AnyAsync(r => r.RoomId == roomId && r.HomeId == home.HomeId);

            if (!roomBelongsToOwner)
                return new List<ESP32Device>();

            return await _context.ESP32Devices
                .Where(e => e.RoomId == roomId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ESP32Device> CreateAsync(CreateESP32DeviceDTO request)
        {
            var roomExists = await _context.Rooms
                .AnyAsync(r => r.RoomId == request.RoomId);

            if (!roomExists)
                throw new Exception("Room not found");

            var macExists = await _context.ESP32Devices
                .AnyAsync(e => e.MacAddress == request.MacAddress);

            if (macExists)
                throw new Exception("MAC address already exists");

            var esp32Device = new ESP32Device
            {
                DeviceName = request.DeviceName,
                MacAddress = request.MacAddress,
                IpAddress = request.IpAddress,
                FirmwareVersion = request.FirmwareVersion,
                ConnectionStatus = request.ConnectionStatus ?? "Offline",
                RoomId = request.RoomId,
                CreatedAt = DateTime.Now
            };

            _context.ESP32Devices.Add(esp32Device);
            await _context.SaveChangesAsync();

            return esp32Device;
        }

        public async Task<bool> UpdateAsync(int id, UpdateESP32DeviceDTO request)
        {
            var esp32Device = await _context.ESP32Devices
                .FirstOrDefaultAsync(e => e.ESP32DeviceId == id);

            if (esp32Device == null)
                return false;

            esp32Device.DeviceName = request.DeviceName;
            esp32Device.IpAddress = request.IpAddress;
            esp32Device.FirmwareVersion = request.FirmwareVersion;
            esp32Device.ConnectionStatus = request.ConnectionStatus;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var esp32Device = await _context.ESP32Devices
                .FirstOrDefaultAsync(e => e.ESP32DeviceId == id);

            if (esp32Device == null)
                return false;

            _context.ESP32Devices.Remove(esp32Device);
            await _context.SaveChangesAsync();

            return true;
        }
    
}
}
