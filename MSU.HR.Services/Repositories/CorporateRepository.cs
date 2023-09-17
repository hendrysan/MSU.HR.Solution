using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MSU.HR.Commons.Extensions;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;

namespace MSU.HR.Services.Repositories
{
    public class CorporateRepository : ICorporate
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly ILogError _logError;
        public CorporateRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogError logError)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            userIdentity = new UserIdentityModel(_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            _logError = logError;
        }

        public async Task<bool> CheckCodeExistsAsync(string code)
        {
            try
            {
                var any = await _context.Corporates.Where(i => i.IsActive == true && i.CorporateCode == code).AnyAsync();

                return any;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { code = code });
                throw new Exception("Bank CheckCodeExistsAsync Error : " + ex.Message);
            }
        }

        public async Task<int> CreateAsync(Corporate entity)
        {
            try
            {
                entity.CreatedBy = userIdentity.Id.ToString();
                entity.CreatedDate = DateTime.Now;

                entity.LastUpdatedBy = userIdentity.Id.ToString();
                entity.LastUpdatedDate = DateTime.Now;
                _context.Corporates.Add(entity);
                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, entity);
                throw new Exception("Corporate Create Error : " + ex.Message);
            }
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _context.Corporates.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    throw new Exception("badrequest Data Not found");

                entity.IsActive = false;
                entity.LastUpdatedBy = userIdentity.Id.ToString();
                entity.LastUpdatedDate = DateTime.Now;

                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { id = id });
                throw new Exception("Corporate Delete Error : " + ex.Message);
            }
        }

        public async Task<Corporate> GetCorporateAsync(Guid id)
        {
            try
            {
                var entity = await _context.Corporates.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();

                return entity;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { id = id });
                throw new Exception("Corporate Find Error : " + ex.Message);
            }
        }

        public async Task<CorporatePagination> GetCorporatesAsync(string search, PaginationModel pagination)
        {
            try
            {
                CorporatePagination result = new CorporatePagination();
                result.Pagination = pagination;
                result.Pagination.TotalRecord = await _context.Corporates.Where(i => i.IsActive == true && i.CorporateCode.Contains(search) || i.Name.Contains(search)).CountAsync();

                var list = await _context.Corporates.Where(i => i.IsActive == true && i.CorporateCode.Contains(search) || i.Name.Contains(search)).Page(pagination.PageNumber, pagination.PageSize).ToListAsync();


                result.Pagination.TotalPage = (int)Math.Ceiling((double)result.Pagination.TotalRecord / pagination.PageSize);
                result.Corporates = list;

                return result;

            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { search = search, pagination = pagination });
                throw new Exception("Corporate Pagination Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<Corporate>> GetCorporatesAsync()
        {
            try
            {
                var list = await _context.Corporates.Where(i => i.IsActive == true).ToListAsync();

                return list;

            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("Corporate Get All Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<DropdownModel>> GetDropdownModelAsync()
        {
            try
            {
                var list = await _context.Corporates.Where(i => i.IsActive == true).ToListAsync();

                var result = list.Select(i => new DropdownModel()
                {
                    Code = i.Id.ToString(),
                    Name = i.Name
                }).ToList();

                return result;

            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("Corporate Dropdown Error : " + ex.Message);
            }
        }

        public async Task<int> UpdateAsync(Guid id, Corporate entity)
        {
            try
            {
                var find = await _context.Corporates.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (find == null)
                    throw new Exception("badrequest Data Not found");

                find.Address1 = entity.Address1;
                find.Address2 = entity.Address2;
                find.Address3 = entity.Address3;
                find.City = entity.City;
                find.Country = entity.Country;
                find.CorporateCode = entity.CorporateCode;
                find.Description = entity.Description;
                find.Email1 = entity.Email1;
                find.Email2 = entity.Email2;
                find.LastUpdatedBy = userIdentity.Id.ToString();
                find.LastUpdatedDate = DateTime.Now;
                find.Logo = entity.Logo;
                find.Name = entity.Name;
                find.Phone1 = entity.Phone1;
                find.Phone2 = entity.Phone2;
                find.State = entity.State;
                find.Website = entity.Website;
                find.ZipCode = entity.ZipCode;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { id = id, entity = entity });
                throw new Exception("Corporate Update Error : " + ex.Message);
            }
        }
    }
}
