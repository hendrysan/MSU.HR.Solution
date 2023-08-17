﻿using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage ="The Code Identity is required")]
        public string? CodeNIK { get; set; }
        [Required]
        public string? Password { get; set; } 
    }
}
