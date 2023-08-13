using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;

namespace MSU.HR.Services.Interfaces
{
    public interface IShift
    {
        Task<ShiftPagination> GetShiftsAsync(string search, PaginationModel pagination);
        Task<IEnumerable<Shift>> GetShiftsAsync();
        Task<Shift> GetShiftAsync(Guid id);
        Task<IEnumerable<DropdownModel>> GetDropdownModelAsync();
        Task<int> CreateAsync(Shift entity);
        Task<int> UpdateAsync(Guid id, Shift entity);
        Task<int> DeleteAsync(Guid id);
        Task<bool> CheckCodeExistsAsync(string code);
    }
}
