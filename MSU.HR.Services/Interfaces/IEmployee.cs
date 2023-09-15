using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;
using MSU.HR.Models.Requests;
using MSU.HR.Models.Responses;

namespace MSU.HR.Services.Interfaces
{
    public interface IEmployee
    {
        Task<EmployeePagination> GetEmployeesAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<Employee> GetEmployeeAsync(Guid id);
        Task<Employee?> GetEmployeeAsync(string code);
        Task<Employee> GetEmployeeAsync();
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(EmployeeRequest request);
        Task<int> UpdateAsync(Guid id, EmployeeRequest request);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);

        Task<DataTableResponse> GetDataTableEmployeeAsync(DataTableRequest request);
    }
}
