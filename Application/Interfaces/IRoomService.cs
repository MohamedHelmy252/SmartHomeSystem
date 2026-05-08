using Application.DTOs.Room;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IRoomService
    {
        Task<List<Room>> GetRoomsByHomeIdAsync(int homeId);
        Task<Room?> GetRoomByIdAsync(int id);
        Task<Room> CreateRoomAsync(CreateRoomDTO request);
        Task<bool> UpdateRoomAsync(int id, UpdateRoomDTO request);
        Task<bool> DeleteRoomAsync(int id);
    }
}
