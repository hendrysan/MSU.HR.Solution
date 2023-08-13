using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IDepartment
    {
        Task<DepartmentPagination> GetDepartmentsAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Department>> GetDepartmentsAsync();
        Task<Department> GetDepartmentAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Department entity);
        Task<int> UpdateAsync(Guid id, Department entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
