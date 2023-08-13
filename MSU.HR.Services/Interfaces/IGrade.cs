using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IGrade
    {
        Task<GradePagination> GetGradesAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Grade>> GetGradesAsync();
        Task<Grade> GetGradeAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Grade entity);
        Task<int> UpdateAsync(Guid id, Grade entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
