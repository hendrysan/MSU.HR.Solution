using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IParameter
    {
        Task<ParameterPagination> GetParametersAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Parameter>> GetParametersAsync();
        Task<Parameter> GetParameterAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Parameter entity);
        Task<int> UpdateAsync(Guid id, Parameter entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
