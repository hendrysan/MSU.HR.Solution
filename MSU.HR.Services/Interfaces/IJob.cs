using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IJob
    {
        Task<JobPagination> GetJobsAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Job>> GetJobsAsync();
        Task<Job> GetJobAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Job entity);
        Task<int> UpdateAsync(Guid id, Job entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
