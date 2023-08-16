using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MSU.HR.Commons.Extensions;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace MSU.HR.Services.Repositories
{
    public class PTKPRepository : IPTKP
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly ILogError _logError;
        public PTKPRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogError logError)
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
                var any = await _context.PTKPs.Where(i => i.IsActive == true && i.Code == code).AnyAsync();

                return any;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(code));
                throw new Exception("Bank CheckCodeExistsAsync Error : " + ex.Message);
            }
        }

        public async Task<int> CreateAsync(PTKP entity)
        {
            try
            {
                entity.CreatedBy = userIdentity.Id.ToString();
                entity.CreatedDate = DateTime.Now;
                entity.IsActive = true;
                entity.LastUpdatedBy = userIdentity.Id.ToString();
                entity.LastUpdatedDate = DateTime.Now;
                _context.PTKPs.Add(entity);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(entity));
                throw new Exception("PTKP Create Error : " + ex.Message);
            }
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _context.PTKPs.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    throw new Exception("badrequest Data Not found");

                entity.IsActive = false;
                entity.LastUpdatedBy = userIdentity.Id.ToString();
                entity.LastUpdatedDate = DateTime.Now;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(id));
                throw new Exception("PTKP Delete Error : " + ex.Message);
            }
        }

        public async Task<PTKP> GetPTKPAsync(Guid id)
        {
            try
            {
                var entity = await _context.PTKPs.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                return entity;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(id));
                throw new Exception("PTKP Find Error : " + ex.Message);
            }
        }

        public async Task<PTKPPagination> GetPTKPsAsync(string search, PaginationModel pagination)
        {
            try
            {
                PTKPPagination result = new PTKPPagination();
                result.Pagination = pagination;
                result.Pagination.TotalRecord = await _context.PTKPs.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).CountAsync();

                var list = await _context.PTKPs.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).Page(pagination.PageNumber, pagination.PageSize).ToListAsync();


                result.Pagination.TotalPage = (int)Math.Ceiling((double)result.Pagination.TotalRecord / pagination.PageSize);
                result.PTKPs = list;

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("PTKP Pagination Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<PTKP>> GetPTKPsAsync()
        {
            try
            {
                var list = await _context.PTKPs.Where(i => i.IsActive == true).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("PTKP Get All Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<DropdownModel>> GetDropdownModelAsync()
        {
            try
            {
                var list = await _context.PTKPs.Where(i => i.IsActive == true).ToListAsync();

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
                throw new Exception("PTKP Dropdown Error : " + ex.Message);
            }
        }

        public async Task<int> UpdateAsync(Guid id, PTKP entity)
        {
            try
            {
                var find = await _context.PTKPs.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (find == null)
                    throw new Exception("badrequest Data Not found");

                find.LastUpdatedBy = userIdentity.Id.ToString();
                find.LastUpdatedDate = DateTime.Now;
                find.Name = entity.Name;
                find.Code = entity.Code;
                find.Description = entity.Description;
                find.Amount = entity.Amount;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("PTKP Update Error : " + ex.Message);
            }
        }
    }
}
