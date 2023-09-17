using Microsoft.EntityFrameworkCore;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Services.Interfaces;

namespace MSU.HR.Services.Repositories
{
    public class AreaRepository : IArea
    {
        private readonly DatabaseContext _context;
        private readonly ILogError _logError;
        public AreaRepository(DatabaseContext context, ILogError logError)
        {
            _context = context;
            _logError = logError;
        }

        public async Task<DropdownModel?> FindName(string code)
        {
            try
            {
                var data = await _context.Areas.Where(i => i.Code == code).FirstOrDefaultAsync();
                if (data == null)
                    return null;

                return new DropdownModel()
                {
                    Code = data.Code,
                    Name = data.Name
                };
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { code = code });
                throw new Exception("Area FindName Error : " + ex.Message);
            }
        }

        public async Task<List<DropdownModel>?> GetProvinces()
        {
            try
            {
                var list = await _context.Areas.Where(i => i.Code.Length == 2).Select(i => new DropdownModel()
                {
                    Code = i.Code,
                    Name = i.Name
                }).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("Area GetProvinces Error : " + ex.Message);
            }
        }

        public async Task<List<DropdownModel>?> GetDistrictsOrCities(string provinceCode)
        {
            try
            {
                var list = await _context.Areas.Where(i => i.Code.Length == 5 && i.Code.Substring(0, 2) == provinceCode).Select(i => new DropdownModel()
                {
                    Code = i.Code,
                    Name = i.Name
                }).ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { provinceCode = provinceCode });
                throw new Exception("Area GetDistrictsOrCities Error : " + ex.Message);
            }
        }

        public async Task<List<DropdownModel>?> GeSubDistricts(string cityCode)
        {
            try
            {
                var list = await _context.Areas.Where(i => i.Code.Length == 8 && i.Code.Substring(0, 5) == cityCode).Select(i => new DropdownModel()
                {
                    Code = i.Code,
                    Name = i.Name
                }).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { cityCode = cityCode });
                throw new Exception("Area GeSubDistricts Error : " + ex.Message);
            }
        }

        public async Task<List<DropdownModel>?> GetVillages(string subDistrictCode)
        {
            try
            {
                var list = await _context.Areas.Where(i => i.Code.Length == 13 && i.Code.Substring(0, 8) == subDistrictCode).Select(i => new DropdownModel()
                {
                    Code = i.Code,
                    Name = i.Name
                }).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { subDistrictCode = subDistrictCode });
                throw new Exception("Area GetVillages Error : " + ex.Message);
            }
        }
    }
}
