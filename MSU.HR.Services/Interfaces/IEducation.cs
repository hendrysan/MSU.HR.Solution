using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IEducation
    {
        Task<EducationPagination> GetEducationsAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Education>> GetEducationsAsync();
        Task<Education> GetEducationAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Education entity);
        Task<int> UpdateAsync(Guid id, Education entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
