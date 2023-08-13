using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface ISection
    {
        Task<SectionPagination> GetSectionsAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Section>> GetSectionsAsync();
        Task<Section> GetSectionAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Section entity);
        Task<int> UpdateAsync(Guid id, Section entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
