using MSU.HR.Models.Others;

namespace MSU.HR.Services.Interfaces
{
    public interface IArea
    {
        Task<List<DropdownModel>?> GetProvinces();
        Task<List<DropdownModel>?> GetDistrictsOrCities(string cityCode);
        Task<List<DropdownModel>?> GeSubDistricts(string provinceCode);
        Task<List<DropdownModel>?> GetVillages(string subDistrictCode);
        Task<DropdownModel?> FindName(string code);
    }
}
