using MSU.HR.Models.Entities;
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
        Task<IEnumerable<TimeOff>> GetTimeOffsAsync(Guid userId);
        Task<TimeOff> GetTimeOffDetailAsync(Guid userId, Guid timeOffId);
        Task<IEnumerable<TimeOffHistory>> GetTimeOffHistoriesAsync(Guid timeOffId);
    }
}
