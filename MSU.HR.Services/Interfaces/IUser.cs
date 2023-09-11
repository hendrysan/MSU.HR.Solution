using MSU.HR.Models.Entities;

namespace MSU.HR.Services.Interfaces
{
    public interface IUser
    {
        Task<int> EmployeeUserConnectedAsync();
        Task<int> EmployeeUserConnectedAsync(string code);
        Task<int> ChangePassword(Guid userId, string hasPassword);
        Task<AspNetUser?> GetProfile();
        Task<AspNetUser?> GetProfile(string code);
        Task<AspNetUser?> GetProfile(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
