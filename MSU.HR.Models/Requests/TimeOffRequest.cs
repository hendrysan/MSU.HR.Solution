using MSU.HR.Commons.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Requests
{
    public class TimeOffRequest
    {
        [Required]
        [BeforeEndDate(EndDatePropertyName = "EndDate")]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public Guid ReasonId { get; set; }
        public string? Notes { get; set; }
        [Required]
        public int Taken { get; set; }
        [Required]
        public int TemporaryAnnualLeaveAllowance { get; set; }
    }

    public class TimeOffActionApproveRequest
    {
        [Required]
        public Guid TimeOffId { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class TimeOffActionRejectRequest
    {
        [Required]
        public Guid TimeOffId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Remarks { get; set; } = string.Empty;
    }

    public class TimeOffActionFinishRequest
    {
        [Required]
        public Guid TimeOffId { get; set; }
    }
}
