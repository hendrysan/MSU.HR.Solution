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
using System.Text.Json;

namespace MSU.HR.Services.Repositories
{
    public class EmployeeRepository : IEmployee
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly ILogError _logError;
        private readonly IBank _bank;
        private readonly IDepartment _department;
        private readonly IEducation _education;
        private readonly IGrade _grade;
        private readonly IJob _job;
        private readonly IPTKP _PTKP;
        private readonly ISection _section;
        private readonly ITypeEmployee _typeEmployee;
        private readonly IUser _user;

        public EmployeeRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogError logError, IBank bank, IDepartment department, IEducation education, IGrade grade,
            IJob job, IPTKP PTKP, ISection section, ITypeEmployee typeEmployee, IUser user)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            userIdentity = new UserIdentityModel(_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity);
            _logError = logError;
            _bank = bank;
            _department = department;
            _education = education;
            _grade = grade;
            _job = job;
            _PTKP = PTKP;
            _section = section;
            _typeEmployee = typeEmployee;
            _user = user;
        }

        public async Task<bool> CheckCodeExistsAsync(string code)
        {
            try
            {
                var any = await _context.Employees.Where(i => i.IsActive == true && i.Code == code).AnyAsync();

                return any;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(code));
                throw new Exception("Bank CheckCodeExistsAsync Error : " + ex.Message);
            }
        }

        public async Task<int> CreateAsync(EmployeeRequest request)
        {
            try
            {
                var bank = await _bank.GetBankAsync(request.BankId);
                var department = await _department.GetDepartmentAsync(request.DepartmentId);
                var education = await _education.GetEducationAsync(request.EducationId);
                var grade = await _grade.GetGradeAsync(request.GradeId);
                var job = await _job.GetJobAsync(request.JobId);
                var ptkp = await _PTKP.GetPTKPAsync(request.PTKPId);
                var section = await _section.GetSectionAsync(request.SectionId);
                var type = await _typeEmployee.GetTypeEmployeeAsync(request.TypeEmployeeId);

                Employee employee = new()
                {
                    Bank = bank,
                    Department = department,
                    Education = education,
                    Grade = grade,
                    Job = job,
                    PTKP = ptkp,
                    Section = section,
                    TypeEmployee = type,
                    Name = request.Name,
                    Code = request.Code,
                    Address = request.Address,
                    BankAccountNumber = request.BankAccountNumber,
                    BaseSalery = request.BaseSalery,
                    BrithDate = request.BrithDate == null ? null : request.BrithDate.Value.Date,
                    City = request.City,
                    Email = request.Email,
                    Gender = request.Gender,
                    GradeAllowance = request.GradeAllowance,
                    Jamsostek = request.Jamsostek,
                    JoinDate = request.JoinDate.Date,
                    MealAllowance = request.MealAllowance,
                    NoIdentity = request.NoIdentity,
                    NPWP = request.NPWP,
                    OtherAllowance = request.OtherAllowance,
                    ResignDate = request.ResignDate == null ? null : request.ResignDate.Value.Date,
                    TransportationAllowance = request.TransportationAllowance,
                    ZipCode = request.ZipCode,
                };

                employee.CreatedBy = userIdentity.Id.ToString();
                employee.CreatedDate = DateTime.Now;
                employee.IsActive = true;
                employee.LastUpdatedBy = userIdentity.Id.ToString();
                employee.LastUpdatedDate = DateTime.Now;
                _context.Employees.Add(employee);
                var result = await _context.SaveChangesAsync();
                await _user.EmployeeUserConnectedAsync(employee.Code);
                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(request));
                throw new Exception("Employee Create Error : " + ex.Message);
            }
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _context.Employees.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
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
                throw new Exception("Employee Delete Error : " + ex.Message);
            }
        }

        public async Task<Employee> GetEmployeeAsync(Guid id)
        {
            try
            {
                var entity = await _context.Employees.Where(i => i.IsActive == true && i.Id == id)
                    .Include(i => i.Bank)
                    .Include(i => i.Department)
                    .Include(i => i.Education)
                    .Include(i => i.Grade)
                    .Include(i => i.Job)
                    .Include(i => i.PTKP)
                    .Include(i => i.Section)
                    .Include(i => i.TypeEmployee)
                    .FirstOrDefaultAsync();
                return entity;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(id));
                throw new Exception("Employee Find Error : " + ex.Message);
            }
        }

        public async Task<Employee> GetEmployeeAsync()
        {
            try
            {
                string code = userIdentity.Code;
                var entity = await _context.Employees.Where(i => i.IsActive == true && i.Code == code)
                    .Include(i => i.Bank)
                    .Include(i => i.Department)
                    .Include(i => i.Education)
                    .Include(i => i.Grade)
                    .Include(i => i.Job)
                    .Include(i => i.PTKP)
                    .Include(i => i.Section)
                    .Include(i => i.TypeEmployee)
                    .FirstOrDefaultAsync();
                return entity;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw new Exception("Employee Find Error : " + ex.Message);
            }
        }

        public async Task<EmployeePagination> GetEmployeesAsync(string search, PaginationModel pagination)
        {
            try
            {
                EmployeePagination result = new EmployeePagination();
                result.Pagination = pagination;
                result.Pagination.TotalRecord = await _context.Employees.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).CountAsync();

                var list = await _context.Employees.Where(i => i.IsActive == true && i.Code.Contains(search) || i.Name.Contains(search)).Page(pagination.PageNumber, pagination.PageSize)
                    .Include(i => i.Bank)
                    .Include(i => i.Department)
                    .Include(i => i.Education)
                    .Include(i => i.Grade)
                    .Include(i => i.Job)
                    .Include(i => i.PTKP)
                    .Include(i => i.Section)
                    .Include(i => i.TypeEmployee)
                    .ToListAsync();


                result.Pagination.TotalPage = (int)Math.Ceiling((double)result.Pagination.TotalRecord / pagination.PageSize);
                result.Employees = list;

                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("Employee Pagination Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            try
            {
                var list = await _context.Employees.Where(i => i.IsActive == true).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, string.Empty);
                throw new Exception("Employee Get All Error : " + ex.Message);
            }
        }

        public async Task<IEnumerable<DropdownModel>> GetDropdownModelAsync()
        {
            try
            {
                var list = await _context.Employees.Where(i => i.IsActive == true).ToListAsync();

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
                throw new Exception("Employee Dropdown Error : " + ex.Message);
            }
        }

        public async Task<int> UpdateAsync(Guid id, EmployeeRequest request)
        {
            try
            {
                var find = await _context.Employees.Where(i => i.IsActive == true && i.Id == id).FirstOrDefaultAsync();
                if (find == null)
                    throw new Exception("badrequest Data Not found");

                var bank = await _bank.GetBankAsync(request.BankId);
                var department = await _department.GetDepartmentAsync(request.DepartmentId);
                var education = await _education.GetEducationAsync(request.EducationId);
                var grade = await _grade.GetGradeAsync(request.GradeId);
                var job = await _job.GetJobAsync(request.JobId);
                var ptkp = await _PTKP.GetPTKPAsync(request.PTKPId);
                var section = await _section.GetSectionAsync(request.SectionId);
                var type = await _typeEmployee.GetTypeEmployeeAsync(request.TypeEmployeeId);

                find.Bank = bank;
                find.Department = department;
                find.Education = education;
                find.Grade = grade;
                find.Job = job;
                find.PTKP = ptkp;
                find.Section = section;
                find.TypeEmployee = type;
                find.Name = request.Name;
                find.Code = request.Code;
                find.Address = request.Address;
                find.BankAccountNumber = request.BankAccountNumber;
                find.BaseSalery = request.BaseSalery;
                find.BrithDate = request.BrithDate == null ? null : request.BrithDate.Value.Date;
                find.City = request.City;
                find.Email = request.Email;
                find.Gender = request.Gender;
                find.GradeAllowance = request.GradeAllowance;
                find.Jamsostek = request.Jamsostek;
                find.JoinDate = request.JoinDate.Date;
                find.MealAllowance = request.MealAllowance;
                find.NoIdentity = request.NoIdentity;
                find.NPWP = request.NPWP;
                find.OtherAllowance = request.OtherAllowance;
                find.ResignDate = request.ResignDate == null ? null : request.ResignDate.Value.Date;
                find.TransportationAllowance = request.TransportationAllowance;
                find.ZipCode = request.ZipCode;
                find.LastUpdatedBy = userIdentity.Id.ToString();
                find.LastUpdatedDate = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                await _user.EmployeeUserConnectedAsync(find.Code);
                return result;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(request));
                throw new Exception("Employee Update Error : " + ex.Message);
            }
        }

        public async Task<Employee?> GetEmployeeAsync(string code)
        {
            var employee = await _context.Employees.Where(i => i.IsActive == true && i.Code == code)
                 .Include(i => i.Bank)
                 .Include(i => i.Department)
                 .Include(i => i.Education)
                 .Include(i => i.Grade)
                 .Include(i => i.Job)
                 .Include(i => i.PTKP)
                 .Include(i => i.Section)
                 .Include(i => i.TypeEmployee)
                 .FirstOrDefaultAsync();
            return employee;
            //if (find == null)
            //    return 0;
        }
    }
}
