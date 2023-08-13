using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface ITypeEmployee
    {
        Task<TypeEmployeePagination> GetTypeEmployeesAsync(string search, PaginationModel pagination);
        Task<IEnumerable<TypeEmployee>> GetTypeEmployeesAsync();
        Task<TypeEmployee> GetTypeEmployeeAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(TypeEmployee entity);
        Task<int> UpdateAsync(Guid id, TypeEmployee entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
