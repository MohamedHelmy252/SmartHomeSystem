using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class KnownFace
    {
        public int FaceId { get; set; }

        public string PersonName { get; set; }

        public string ImagePath { get; set; }

        public string RelationToOwner { get; set; }

        public int HomeId { get; set; }

        public DateTime CreatedAt { get; set; }

        #region Relation
        public Home Home { get; set; }
        #endregion
    }
}