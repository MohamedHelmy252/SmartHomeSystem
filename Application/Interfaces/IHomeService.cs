using Application.DTOs.Home;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IHomeService
    {
        Task<List<Home>> GetAllHomesAsync();
        Task<Home?> GetHomeByIdAsync(int id);
        Task<Home?> GetHomeByOwnerIdAsync(int ownerUserId);
        Task<Home> CreateHomeAsync(CreateHomeDTO request);
        Task<bool> UpdateHomeAsync(int id, UpdateHomeDTO request);
        Task<bool> DeleteHomeAsync(int id);
    }
}
