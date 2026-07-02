using Application.DTOs.FaceRecognition;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IFaceMemberService
    {
        Task<HomeMemberFaceResponseDTO> CreateAsync(CreateHomeMemberFaceDTO dto);

        Task<List<HomeMemberFaceResponseDTO>> GetByHomeAsync(int homeId);

        Task<bool> DeleteAsync(int faceId);
    }
}
