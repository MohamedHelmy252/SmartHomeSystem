using Application.DTOs.Sensor;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ISensorService
    {
        Task<List<Sensor>> GetAllAsync();
        Task<Sensor?> GetByIdAsync(int id);
        Task<List<Sensor>> GetByESP32IdAsync(int esp32Id);

        Task<List<Sensor>> GetForHomeOwnerAsync(int userId);
        Task<List<Sensor>> GetForHomeOwnerRoomAsync(int userId, int roomId);

        Task<Sensor> CreateAsync(CreateSensorDTO request);
        Task<bool> UpdateAsync(int id, UpdateSensorDTO request);
        Task<bool> DeleteAsync(int id);
    }
}
