using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;
using MSU.HR.Models.Requests;

namespace MSU.HR.Services.Interfaces
{
    public interface ISection
    {
        Task<SectionPagination> GetSectionsAsync(Guid departmentId, string search, PaginationModel pagination);
        Task<IEnumerable<Section>> GetSectionsAsync(Guid departmentId);
        Task<Section> GetSectionAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync(Guid departmentId);
        Task<int> CreateAsync(SectionRequest entity);
        Task<int> UpdateAsync(Guid id, SectionRequest entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
