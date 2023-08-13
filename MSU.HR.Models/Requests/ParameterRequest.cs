using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Requests
{
    public class ParameterRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string? Code { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string? Name { get; set; }

        public decimal? Amount { get; set; }
    }
}
