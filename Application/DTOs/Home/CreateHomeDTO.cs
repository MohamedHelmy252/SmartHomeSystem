using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Home
{
    public class CreateHomeDTO
    {
        [Required]
        public string HomeName { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        [Required]
        public int OwnerUserId { get; set; }
    }
}
