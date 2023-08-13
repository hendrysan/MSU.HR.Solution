using MSU.HR.Models.Entities;

namespace MSU.HR.Services.Interfaces
{
    public interface IUser
    {
        Task<int> EmployeeUserConnectedAsync();
        Task<int> ChangePassword();
        Task<AspNetUser?> GetProfile();
        Task<AspNetUser?> GetProfile(string code);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
