using MSU.HR.Commons.Enums;

namespace MSU.HR.Models.Others
{
    public class TimeOffSummaryModel
    {
        public int Year { get; set; }
        public string UserCode { get; set; }
        public int ReminingBalance { get; set; }
        public int TotalTaken { get; set; } 
        public List<string> Status { get; set; }
        public List<TimeOffSummaryDetailModel>? Details { get; set; }
    }

    public class TimeOffSummaryDetailModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public int Taken { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Approver { get; set; }
        public string Remarks { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string ReasonName { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }
}
