using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Camera
    {
        public int CameraId { get; set; }

        public string CameraName { get; set; }

        public string IpAddress { get; set; }

        public string StreamUrl { get; set; }

        public string Status { get; set; }

        public int HomeId { get; set; }

        public DateTime CreatedAt { get; set; }
        #region Relation 
        public Home Home { get; set; }

        #endregion
    }
}