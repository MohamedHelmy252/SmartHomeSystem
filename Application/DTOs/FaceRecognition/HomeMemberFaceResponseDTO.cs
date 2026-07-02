using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.FaceRecognition
{
    public class HomeMemberFaceResponseDTO
    {
        public int FaceId { get; set; }

        public int HomeId { get; set; }

        public string PersonName { get; set; }

        public string ImagePath { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
