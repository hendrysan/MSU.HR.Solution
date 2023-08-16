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
    public class JobRepository : IJob
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly ILogError _logError;
        public JobRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogError logError)
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
                var any = await _context.Jobs.Where(i => i.IsActive == true && i.Code == code).AnyAsync();

                return any;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(code));
                throw new Exception("Bank CheckCodeExistsAsync Error : " + ex.Message);
            }
        }

        public async Task<int> CreateAsync(Job entity)
        {
            try
            {
                entity.CreatedBy = userIdentity.Id.ToString();
                entity.CreatedDate = DateTime.Now;
                entity.IsActive = true;
                entity.LastUpdatedBy = userIdentity.Id.ToString();
                entity.LastUpdatedDate = DateTime.Now;
                _context.Jobs.Add(entity);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(entity));
                throw new Exception("Job Create Error : " + ex.Message);
            }
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _context.Jobs.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
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
                throw new Exception("Job Delete Error : " + ex.Message);
            }
        }

        public async Task<Job> GetJobAsync(Guid id)
        {
            try
            {
                var entity = await _context.Jobs.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                return entity;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(id));
                throw new Exception("Job Find Error : " + ex.Message);
            }
        }

        public async Task<JobPagination> GetJobsAsync(string search, PaginationModel pagination)
        {
            try
            {
                JobPagination result = new JobPagination();
                result.Pagination = pagination;
                result.Pagination.TotalRecord = await _context.Jobs.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).CountAsync();

                var list = await _context.Jobs.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).Page(pagination.PageNumber, pagination.PageSize).ToListAsync();


                result.Pagination.TotalPage = (int)Math.Ceiling((double)result.Pagination.TotalRecord / pagination.PageSize);
                result.Jobs = list;

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("Job Pagination Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<Job>> GetJobsAsync()
        {
            try
            {
                var list = await _context.Jobs.Where(i => i.IsActive == true).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("Job Get All Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<DropdownModel>> GetDropdownModelAsync()
        {
            try
            {
                var list = await _context.Jobs.Where(i => i.IsActive == true).ToListAsync();

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
                throw new Exception("Job Dropdown Error : " + ex.Message);
            }
        }

        public async Task<int> UpdateAsync(Guid id, Job entity)
        {
            try
            {
                var find = await _context.Jobs.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (find == null)
                    throw new Exception("badrequest Data Not found");

                find.LastUpdatedBy = userIdentity.Id.ToString();
                find.LastUpdatedDate = DateTime.Now;
                find.Name = entity.Name;
                find.Code = entity.Code;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("Job Update Error : " + ex.Message);
            }
        }
    }
}
