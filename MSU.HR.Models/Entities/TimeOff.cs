using System.ComponentModel.DataAnnotations;

namespace MSU.HR.Models.Entities
{
    public class TimeOff
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string UserCode { get; set; }
        public Reason? Reason { get; set; }
        public int TemporaryAnnualLeaveAllowance { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [StringLength(150)]
        public string? Notes { get; set; }
        public int Taken { get; set; }
        [StringLength(50)]
        public string StatusId { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        [StringLength(150)]
        public string? ApprovedRemarks { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
