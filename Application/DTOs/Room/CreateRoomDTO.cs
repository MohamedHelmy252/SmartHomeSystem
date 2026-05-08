using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Room
{
    public class CreateRoomDTO
    {
        [Required]
        public string RoomName { get; set; }

        [Required]
        public int HomeId { get; set; }
    }
}
