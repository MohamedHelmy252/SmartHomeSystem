using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.User
{
    public class UpdateUserRoleDTO
    {
        [Required]
        public string Role { get; set; }
    }
}
