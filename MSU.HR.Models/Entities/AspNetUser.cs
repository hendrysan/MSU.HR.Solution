using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Entities
{
    public class AspNetUser : IdentityUser
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Code { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string CreatedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string LastModifiedBy { get; set; }

        public Role? Role { get; set; }
        public Corporate? Corporate { get; set; }
        public Employee? Employee { get; set; }

        public bool IsConnected { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
