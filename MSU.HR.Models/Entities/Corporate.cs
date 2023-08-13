using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Entities
{
    public class Corporate
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string CorporateCode { get; set; } = string.Empty;

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Address1 { get; set; } = string.Empty;

        [StringLength(250, MinimumLength = 3)]
        public string? Address2 { get; set; }

        [StringLength(250, MinimumLength = 3)]
        public string? Address3 { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Email1 { get; set; } = string.Empty;

        [StringLength(150, MinimumLength = 3)]
        public string? Email2 { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Phone1 { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 3)]
        public string? Phone2 { get; set; }

        [StringLength(500, MinimumLength = 3)]
        public string? Logo { get; set; }

        [StringLength(250, MinimumLength = 3)]
        public string? Website { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Description { get; set; } = string.Empty;

        [StringLength(150, MinimumLength = 3)]
        public string? Country { get; set; }

        [StringLength(150, MinimumLength = 3)]
        public string? State { get; set; }

        [StringLength(150, MinimumLength = 3)]
        public string? City { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? ZipCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime LastUpdatedDate { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string LastUpdatedBy { get; set; } = string.Empty;
    }
}
