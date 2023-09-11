using MSU.HR.Models.Entities;

namespace MSU.HR.Models.ViewModels
{
    public class AttendanceDocumentDetailViewModel
    {
        public Guid Id { get; set; }
        public DocumentAttendance DocumentAttendance { get; set; }
        public AspNetUser User { get; set; }
        public string Action { get; set; }
    }
}
