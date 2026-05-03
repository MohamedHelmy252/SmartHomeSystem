using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[&%$#]).{8,}$",
         ErrorMessage = "Password must contain letters, numbers, and special characters (& % $ #)")]
  
        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string? TwoFactorCode { get; set; }
        public DateTime? TwoFactorCodeExpiresAt { get; set; }

        #region Relation
        public Home Home { get; set; }

        public ICollection<AutomationRule> AutomationRules { get; set; }
        public ICollection<Log> Logs { get; set; }
        #endregion
    }
}