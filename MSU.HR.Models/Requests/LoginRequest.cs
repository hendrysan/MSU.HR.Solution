using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Requests
{
    public class LoginRequest
    {
        [Required]
        public string? CodeNIK { get; set; }
        [Required]
        public string? Password { get; set; } 
    }
}
