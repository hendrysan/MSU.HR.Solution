using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MSU.HR.Commons.Extensions;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Paginations;
using MSU.HR.Models.Requests;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;

namespace MSU.HR.Services.Repositories
{
    public class SectionRepository : ISection
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly ILogError _logError;
        private readonly IDepartment _department;
        public SectionRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogError logError, IDepartment department)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            userIdentity = new UserIdentityModel(_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            _logError = logError;
            _department = department;
        }

        public async Task<bool> CheckCodeExistsAsync(string code)
        {
            try
            {
                var any = await _context.Sections.Where(i => i.IsActive == true && i.Code == code).AnyAsync();

                return any;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { code = code });
                throw new Exception("Section CheckCodeExistsAsync Error : " + ex.Message);
            }
        }

        public async Task<int> CreateAsync(SectionRequest request)
        {
            try
            {
                Section entity = new Section();
                var department = await _department.GetDepartmentAsync(request.DepartmentId);

                entity.Id = Guid.NewGuid();
                entity.Code = request.Code;
                entity.Name = request.Name;
                entity.Department = department;
                entity.CreatedBy = userIdentity.Id.ToString();
                entity.CreatedDate = DateTime.Now;
                entity.IsActive = true;
                entity.LastUpdatedBy = userIdentity.Id.ToString();
                entity.LastUpdatedDate = DateTime.Now;
                _context.Sections.Add(entity);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, request);
                throw new Exception("Section Create Error : " + ex.Message);
            }
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _context.Sections.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
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
                throw new Exception("Section Delete Error : " + ex.Message);
            }
        }

        public async Task<Section> GetSectionAsync(Guid id)
        {
            try
            {
                var entity = await _context.Sections.Where(i => i.IsActive == true && i.Id == id).Include(d => d.Department).FirstOrDefaultAsync();
                return entity;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { id = id });
                throw new Exception("Section Find Error : " + ex.Message);
            }
        }

        public async Task<SectionPagination> GetSectionsAsync(Guid departmentId, string search, PaginationModel pagination)
        {
            try
            {
                SectionPagination result = new SectionPagination();
                result.Pagination = pagination;

                if (departmentId == Guid.Empty)
                {
                    result.Pagination.TotalRecord = await _context.Sections.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).CountAsync();
                    result.Sections = await _context.Sections.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).Page(pagination.PageNumber, pagination.PageSize).Include(d => d.Department).ToListAsync();
                }
                else
                {
                    var department = await _department.GetDepartmentAsync(departmentId);
                    result.Pagination.TotalRecord = await _context.Sections.Where(i => i.IsActive == true && i.Department == department && i.Code.Contains(search) || i.Name.Contains(search)).CountAsync();
                    result.Sections = await _context.Sections.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).Page(pagination.PageNumber, pagination.PageSize).ToListAsync();
                }


                result.Pagination.TotalPage = (int)Math.Ceiling((double)result.Pagination.TotalRecord / pagination.PageSize);

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { departmentId = departmentId, search = search, pagination = pagination });
                throw new Exception("Section Pagination Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<Section>> GetSectionsAsync(Guid departmentId)
        {
            try
            {
                var list = await _context.Sections.Where(i => i.IsActive == true && i.Department.Id == departmentId).ToListAsync();
                return list;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { departmentId = departmentId });
                throw new Exception("Section Get All Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<DropdownModel>> GetDropdownModelAsync(Guid departmentId)
        {
            try
            {
                var list = await _context.Sections.Where(i => i.IsActive == true && i.Department.Id == departmentId).ToListAsync();

                var result = list.Select(i => new DropdownModel()
                {
                    Code = i.Id.ToString(),
                    Name = i.Name
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { departmentId = departmentId });
                throw new Exception("Section Dropdown Error : " + ex.Message);
            }
        }

        public async Task<int> UpdateAsync(Guid id, SectionRequest request)
        {
            try
            {
                var find = await _context.Sections.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (find == null)
                    throw new Exception("badrequest Data Not found");

                var department = await _department.GetDepartmentAsync(request.DepartmentId);

                find.LastUpdatedBy = userIdentity.Id.ToString();
                find.LastUpdatedDate = DateTime.Now;
                find.Name = request.Name;
                find.Code = request.Code;

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, new { id = id, request = request });
                throw new Exception("Section Update Error : " + ex.Message);
            }
        }
    }
}
