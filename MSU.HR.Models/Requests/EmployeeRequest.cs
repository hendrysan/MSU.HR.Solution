using MSU.HR.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Requests
{
    public class EmployeeRequest
    {
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
        [Required]
        public string Email { get; set; }
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
        public Guid BankId { get; set; }
        [Required]
        public Guid DepartmentId { get; set; }
        [Required]
        public Guid EducationId { get; set; }
        [Required]
        public Guid GradeId { get; set; }
        [Required]
        public Guid JobId { get; set; }
        [Required]
        public Guid PTKPId { get; set; }
        [Required]
        public Guid SectionId { get; set; }
        [Required]
        public Guid TypeEmployeeId { get; set; }

        public decimal BaseSalery { get; set; }
        public decimal GradeAllowance { get; set; }
        public decimal MealAllowance { get; set; }
        public decimal TransportationAllowance { get; set; }
        public decimal OtherAllowance { get; set; }
    }
}
