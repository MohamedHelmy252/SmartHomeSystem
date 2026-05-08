using Application.DTOs.Room;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;

        public RoomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetRoomsByHomeIdAsync(int homeId)
        {
            return await _context.Rooms
                .Where(r => r.HomeId == homeId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            return await _context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoomId == id);
        }

        public async Task<Room> CreateRoomAsync(CreateRoomDTO request)
        {
            var room = new Room
            {
                Name = request.RoomName,
                HomeId = request.HomeId,
                CreatedAt = DateTime.Now
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return room;
        }

        public async Task<bool> UpdateRoomAsync(int id, UpdateRoomDTO request)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return false;

            room.Name = request.RoomName;
         

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return false;

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return true;
        }
    }

}
