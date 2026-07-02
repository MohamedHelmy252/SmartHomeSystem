using Application.DTOs.FaceRecognition;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Infrastructure.Services
{
    public class FaceMemberService : IFaceMemberService
    {
        private readonly AppDbContext _context;

        public FaceMemberService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HomeMemberFaceResponseDTO> CreateAsync(CreateHomeMemberFaceDTO dto)
        {
            var homeExists = await _context.Homes
                .AnyAsync(h => h.HomeId == dto.HomeId);

            if (!homeExists)
                throw new Exception("Home not found");

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);

            var folder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "FaceImages"
            );

            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            var face = new HomeMemberFace
            {
                HomeId = dto.HomeId,
                PersonName = dto.PersonName,
                ImagePath = $"FaceImages/{fileName}",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.HomeMemberFaces.Add(face);
            await _context.SaveChangesAsync();

            return new HomeMemberFaceResponseDTO
            {
                FaceId = face.FaceId,
                HomeId = face.HomeId,
                PersonName = face.PersonName,
                ImagePath = face.ImagePath,
                IsActive = face.IsActive,
                CreatedAt = face.CreatedAt
            };
        }

        public async Task<List<HomeMemberFaceResponseDTO>> GetByHomeAsync(int homeId)
        {
            return await _context.HomeMemberFaces
                .Where(f => f.HomeId == homeId && f.IsActive)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new HomeMemberFaceResponseDTO
                {
                    FaceId = f.FaceId,
                    HomeId = f.HomeId,
                    PersonName = f.PersonName,
                    ImagePath = f.ImagePath,
                    IsActive = f.IsActive,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int faceId)
        {
            var face = await _context.HomeMemberFaces
                .FirstOrDefaultAsync(f => f.FaceId == faceId);

            if (face == null)
                return false;

            face.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}