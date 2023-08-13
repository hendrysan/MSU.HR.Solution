using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Entities
{
    public class Section
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Code { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Name { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime LastUpdatedDate { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string LastUpdatedBy { get; set; } = string.Empty;

    }
}
