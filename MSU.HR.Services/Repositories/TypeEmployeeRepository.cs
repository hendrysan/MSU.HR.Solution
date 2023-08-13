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
    public class TypeEmployeeRepository : ITypeEmployee
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly ILogError _logError;
        public TypeEmployeeRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogError logError)
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
                var any = await _context.TypeEmployees.Where(i => i.IsActive == true && i.Code == code).AnyAsync();

                return any;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(code));
                throw new Exception("Bank CheckCodeExistsAsync Error : " + ex.Message);
            }
        }

        public async Task<int> CreateAsync(TypeEmployee entity)
        {
            try
            {
                entity.CreatedBy = userIdentity.Id.ToString();
                entity.CreatedDate = DateTime.Now;
                entity.IsActive = true;
                entity.LastUpdatedBy = userIdentity.Id.ToString();
                entity.LastUpdatedDate = DateTime.Now;
                _context.TypeEmployees.Add(entity);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(entity));
                throw new Exception("TypeEmployee Create Error : " + ex.Message);
            }
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _context.TypeEmployees.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (entity == null)
                    return 0;

                entity.IsActive = false;
                entity.LastUpdatedBy = userIdentity.Id.ToString();
                entity.LastUpdatedDate = DateTime.Now;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(id));
                throw new Exception("TypeEmployee Delete Error : " + ex.Message);
            }
        }

        public async Task<TypeEmployee> GetTypeEmployeeAsync(Guid id)
        {
            try
            {
                var entity = await _context.TypeEmployees.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                return entity;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(id));
                throw new Exception("TypeEmployee Find Error : " + ex.Message);
            }
        }

        public async Task<TypeEmployeePagination> GetTypeEmployeesAsync(string search, PaginationModel pagination)
        {
            try
            {
                TypeEmployeePagination result = new TypeEmployeePagination();
                result.Pagination = pagination;
                result.Pagination.TotalRecord = await _context.TypeEmployees.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).CountAsync();

                var list = await _context.TypeEmployees.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).Page(pagination.PageNumber, pagination.PageSize).ToListAsync();


                result.Pagination.TotalPage = (int)Math.Ceiling((double)result.Pagination.TotalRecord / pagination.PageSize);
                result.TypeEmployees = list;

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("TypeEmployee Pagination Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<TypeEmployee>> GetTypeEmployeesAsync()
        {
            try
            {
                var list = await _context.TypeEmployees.Where(i => i.IsActive == true).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("TypeEmployee Get All Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<DropdownModel>> GetDropdownModelAsync()
        {
            try
            {
                var list = await _context.TypeEmployees.Where(i => i.IsActive == true).ToListAsync();

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
                throw new Exception("TypeEmployee Dropdown Error : " + ex.Message);
            }
        }

        public async Task<int> UpdateAsync(Guid id, TypeEmployee entity)
        {
            try
            {
                var find = await _context.TypeEmployees.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (find == null)
                    return 0;

                find.LastUpdatedBy = userIdentity.Id.ToString();
                find.LastUpdatedDate = DateTime.Now;
                find.Name = entity.Name;
                find.Code = entity.Code;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("TypeEmployee Update Error : " + ex.Message);
            }
        }
    }
}
