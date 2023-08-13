using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Entities
{
    public class RoleAccess
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        public Role Role { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Code { get; set; }

        [StringLength(250, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(250, MinimumLength = 3)]
        public string Description { get; set; } = string.Empty;

        [StringLength(250, MinimumLength = 3)]
        public string URL { get; set; } = string.Empty;

        [Required]
        public bool IsView { get; set; } = false;
        [Required]
        public bool IsAdd { get; set; } = false;
        [Required]
        public bool IsEdit { get; set; } = false;
        [Required]
        public bool IsDelete { get; set; } = false;
        [Required]
        public bool IsPrint { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        [StringLength(50, MinimumLength = 3)] 
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime LastUpdatedDate { get; set; } = DateTime.Now;
        [StringLength(50, MinimumLength = 3)]
        public string LastUpdatedBy { get; set; } = string.Empty;

    }
}
