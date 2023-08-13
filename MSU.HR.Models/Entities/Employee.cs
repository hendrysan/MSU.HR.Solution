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

        [Required]
        public DateTime? BrithDate { get; set; }

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

        [Required]
        public Bank Bank { get; set; }
        [Required]
        public Department Department { get; set; }
        [Required]
        public Education Education { get; set; }
        [Required]
        public Grade Grade { get; set; }
        [Required]
        public Job Job { get; set; }
        [Required]
        public PTKP PTKP { get; set; }
        [Required]
        public Section Section { get; set; }
        [Required]
        public TypeEmployee TypeEmployee { get; set; }
        
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
