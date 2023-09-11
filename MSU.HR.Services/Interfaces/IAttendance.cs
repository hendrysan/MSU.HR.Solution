using Microsoft.AspNetCore.Http;

namespace MSU.HR.Services.Interfaces
{
    public interface IAttendance
    {
        Task<int> UploadAsync(IFormFile file);
        //Task<int> CreateAsync();
        //Task<int> UpdateAsync();
        //Task<int> DeleteAsync();
        //Task<int> GetAsync(string code);
    }
}
