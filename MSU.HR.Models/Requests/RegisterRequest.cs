namespace MSU.HR.Models.Requests
{
    public class RegisterRequest
    {
        public string? CodeNIK { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }        

    }
}
