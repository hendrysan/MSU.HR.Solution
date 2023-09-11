using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSU.HR.Models.Entities
{
    public class DocumentAttendance
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string DocumentName { get; set; }
        public string Path { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Type { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Extension { get; set; }
        [Required]
        public string Status { get; set; }
        public string? Remarks { get; set; }
        [Required]
        public Guid CreatedBy { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }

        public List<DocumentAttendanceDetail>? Details { get; set; }
    }
}
