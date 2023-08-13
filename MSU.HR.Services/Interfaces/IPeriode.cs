using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IPeriod
    {
        Task<PeriodPagination> GetPeriodsAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Period>> GetPeriodsAsync();
        Task<Period> GetPeriodAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Period entity);
        Task<int> UpdateAsync(Guid id, Period entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
