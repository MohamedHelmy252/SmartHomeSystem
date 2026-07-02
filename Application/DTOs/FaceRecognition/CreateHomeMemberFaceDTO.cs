using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
namespace Application.DTOs.FaceRecognition
{
    public class CreateHomeMemberFaceDTO
    {
        public int HomeId { get; set; }

        public string PersonName { get; set; }
        public IFormFile Image { get; set; } = null!;

    }

}
