using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IRole
    {
        Task<RolePagination> GetRolesAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<Role> GetRoleAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Role entity);
        Task<int> UpdateAsync(Guid id, Role entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
