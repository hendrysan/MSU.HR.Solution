using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Entities
{
    public class Period
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Code { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string MonthName { get; set; }

        [Required]
        public int LastDay { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime LastUpdatedDate { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string LastUpdatedBy { get; set; } = string.Empty;

    }
}
