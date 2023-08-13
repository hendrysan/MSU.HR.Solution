using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IBank
    {
        Task<BankPagination> GetBanksAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Bank>> GetBanksAsync();
        Task<Bank> GetBankAsync(Guid id);        
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Bank entity);
        Task<int> UpdateAsync(Guid id, Bank entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
