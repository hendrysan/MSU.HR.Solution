using MSU.HR.Models.Entities;

namespace MSU.HR.Models.Responses
{
    public class TimeOffDetailResponse
    {
        public TimeOff TimeOff { get; set; }
        public IEnumerable<TimeOffHistory> TimeOffHistories { get; set; }
    }
}
