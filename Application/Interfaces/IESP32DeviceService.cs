using Application.DTOs.ESP32Device;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IESP32DeviceService
    {
        Task<List<ESP32Device>> GetAllAsync();
        Task<ESP32Device?> GetByIdAsync(int id);
        Task<List<ESP32Device>> GetByRoomIdAsync(int roomId);
        Task<List<ESP32Device>> GetForHomeOwnerAsync(int userId);
        Task<List<ESP32Device>> GetForHomeOwnerRoomAsync(int userId, int roomId);
        Task<ESP32Device> CreateAsync(CreateESP32DeviceDTO request);
        Task<bool> UpdateAsync(int id, UpdateESP32DeviceDTO request);
        Task<bool> DeleteAsync(int id);
    }
}
