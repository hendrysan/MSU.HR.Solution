using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Requests
{
    public class RoleRequest  
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
