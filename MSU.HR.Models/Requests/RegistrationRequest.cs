using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Requests
{
    public class RegistrationRequest
    {
        [Required]
        public string CodeNIK { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string FullName { get; set; } = null!;
    }
}
