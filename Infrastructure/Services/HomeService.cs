using Application.DTOs.Home;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class HomeService: IHomeService
    {
        private readonly AppDbContext _context;

        public HomeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Home>> GetAllHomesAsync()
        {
            return await _context.Homes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Home?> GetHomeByIdAsync(int id)
        {
            return await _context.Homes
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.HomeId == id);
        }

        public async Task<Home?> GetHomeByOwnerIdAsync(int ownerUserId)
        {
            return await _context.Homes
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.OwnerUserId == ownerUserId);
        }

        public async Task<Home> CreateHomeAsync(CreateHomeDTO request)
        {
            var ownerExists = await _context.Users
                .AnyAsync(u => u.UserId == request.OwnerUserId);

            if (!ownerExists)
                throw new Exception("Owner user not found");

            var home = new Home
            {
                HomeName = request.HomeName,
                Address = request.Address,
                City = request.City,
                Country = request.Country,
                OwnerUserId = request.OwnerUserId,
                CreatedAt = DateTime.Now
            };

            _context.Homes.Add(home);
            await _context.SaveChangesAsync();

            return home;
        }

        public async Task<bool> UpdateHomeAsync(int id, UpdateHomeDTO request)
        {
            var home = await _context.Homes
                .FirstOrDefaultAsync(h => h.HomeId == id);

            if (home == null)
                return false;

            home.HomeName = request.HomeName;
            home.Address = request.Address;
            home.City = request.City;
            home.Country = request.Country;
            home.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteHomeAsync(int id)
        {
            var home = await _context.Homes
                .FirstOrDefaultAsync(h => h.HomeId == id);

            if (home == null)
                return false;

            _context.Homes.Remove(home);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
