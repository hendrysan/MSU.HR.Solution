using MSU.HR.Models.Entities;

namespace MSU.HR.Models.ViewModels
{
    public class PaidLeaveIndexModel
    {
        public int ReminingAllowance { get; set; }
        public IEnumerable<TimeOff>? TimeOffs { get; set; }
    }
}
