using Application.DTOs.SmartDevice;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ISmartDeviceService
    {
        Task<List<SmartDevice>> GetAllAsync();
        Task<SmartDevice?> GetByIdAsync(int id);
        Task<List<SmartDevice>> GetByESP32IdAsync(int esp32Id);

        Task<List<SmartDevice>> GetForHomeOwnerAsync(int userId);
        Task<List<SmartDevice>> GetForHomeOwnerRoomAsync(int userId, int roomId);

        Task<SmartDevice> CreateAsync(CreateSmartDeviceDTO request);
        Task<bool> UpdateAsync(int id, UpdateSmartDeviceDTO request);
        Task<bool> DeleteAsync(int id);

        Task<SmartDevice?> ControlAsync(int userId, int deviceId, string state);
    }
}
