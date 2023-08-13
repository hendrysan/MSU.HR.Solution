using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Entities
{
    public class LogError
    {
        public Guid Id { get; set; }
        public string? Message { get; set; }
        public string? Type { get; set; }
        public string? Source { get; set; }
        public string? URL { get; set; }
        
        [StringLength(50, MinimumLength = 3)]
        public string? IP { get; set; }
        
        [StringLength(50, MinimumLength = 3)]
        public string? IPClient { get; set; }
        public string? Browser { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? UserAgent { get; set; }
        public string? Parameter { get; set; }
        public string? ParameterBody { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
