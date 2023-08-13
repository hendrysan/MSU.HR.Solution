using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface ICorporate
    {
        Task<CorporatePagination> GetCorporatesAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Corporate>> GetCorporatesAsync();
        Task<Corporate> GetCorporateAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Corporate entity);
        Task<int> UpdateAsync(Guid id, Corporate entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
