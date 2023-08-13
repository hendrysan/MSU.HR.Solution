using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IPTKP
    {
        Task<PTKPPagination> GetPTKPsAsync(string search, PaginationModel pagination);
        Task<IEnumerable<PTKP>> GetPTKPsAsync();
        Task<PTKP> GetPTKPAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(PTKP entity);
        Task<int> UpdateAsync(Guid id, PTKP entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
