using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Room
{
    public class UpdateRoomDTO
    {
        [Required]
        public string RoomName { get; set; }
    }
}
