using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Requests;
using MSU.HR.Models.Responses;

namespace MSU.HR.Services.Interfaces
{
    public interface ITimeOff
    {
        Task<CountLeaveAllowanceResponse> GetCountLeaveAllowanceAsync(Guid userId);
        Task<int> SubmitAsync(TimeOffRequest request);
        Task<int> ApproveAsync(Guid timeOffId, string remarks);
        Task<int> RejectAsync(Guid timeOffId, string remarks);
        Task<int> FinishAsync(Guid timeOffId);
        Task<int> ExpiredAsync(Guid userId);
        Task<Employee?> GetUserSuperiorityAsync(string code);
        Task<IEnumerable<TimeOff>?> GetTimeOffsAsync(Guid userId);
        Task<IEnumerable<TimeOff>?> GetPendingApprovalTimeOffsAsync(string code);
        Task<IEnumerable<TimeOff>?> GetPendingFinishTimeOffsAsync();
        Task<TimeOff> GetTimeOffDetailAsync(Guid timeOffId);
        Task<IEnumerable<TimeOffHistory>?> GetTimeOffHistoriesAsync(Guid timeOffId);
        Task<TimeOffSummaryModel?> GetTimeOffSummaryAsync(string code, int year);
    }
}
