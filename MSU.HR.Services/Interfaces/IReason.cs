using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IReason
    {
        Task<ReasonPagination> GetReasonsAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Reason>> GetReasonsAsync();
        Task<Reason> GetReasonAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Reason entity);
        Task<int> UpdateAsync(Guid id, Reason entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
