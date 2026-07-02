using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class HomeMemberFace
    {
        [Key]
        public int FaceId { get; set; }

        public int HomeId { get; set; }

        public string PersonName { get; set; }

        public string ImagePath { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Home Home { get; set; }

      
    }
}
