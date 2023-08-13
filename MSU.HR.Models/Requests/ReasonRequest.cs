using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Requests
{
    public class ReasonRequest  
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Code { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Name { get; set; }
        [StringLength(150, MinimumLength = 3)]

        public string Description { get; set; }
    }
}
