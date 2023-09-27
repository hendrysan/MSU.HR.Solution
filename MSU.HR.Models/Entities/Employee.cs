using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Entities
{
    public class Employee
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

        [Required]
        public DateTime JoinDate { get; set; }

        public DateTime? ResignDate { get; set; }

        public string? PlaceOfBirth { get; set; }

        [Required]
        public DateTime? BrithDate { get; set; }

        public string? Phone { get; set; }
        public string? Religion { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Address { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string City { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string ZipCode { get; set; }

        [Required]
        public int Gender { get; set; }

        public string? Email { get; set; }

        [Required]
        public string NPWP { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Jamsostek { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string NoIdentity { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string BankAccountNumber { get; set; }

        
        public Bank? Bank { get; set; }
        
        public Department? Department { get; set; }
        
        public Education? Education { get; set; }
        
        public Grade? Grade { get; set; }
        
        public Job? Job { get; set; }
        
        public PTKP? PTKP { get; set; }
        
        public Section? Section { get; set; }
        
        public TypeEmployee? TypeEmployee { get; set; }
        
        public decimal BaseSalery { get; set; }

        public decimal GradeAllowance { get; set; }
        
        public decimal MealAllowance { get; set; }
        
        public decimal TransportationAllowance { get; set; }
        
        public decimal OtherAllowance { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime LastUpdatedDate { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string LastUpdatedBy { get; set; } = string.Empty;
    }
}
