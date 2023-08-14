using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace MSU.HR.Services.Repositories
{
    public class UserRepository : IUser
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserIdentityModel userIdentity;
        private readonly ILogError _logError;

        public UserRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogError logError)
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
                var any = await _context.Users.Where(i => i.IsActive == true && i.Code == code).AnyAsync();

                return any;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(code));
                throw new Exception("Bank CheckCodeExistsAsync Error : " + ex.Message);
            }
        }


        public async Task<int> ChangePassword(Guid userId, string hasPassword)
        {
            var user = await _context.Users.Where(i => i.IsActive == true && i.Id == userId.ToString()).FirstOrDefaultAsync();
            if (user == null)
                return 0;

            user.PasswordHash = hasPassword;
            user.LastModifiedBy = userIdentity.Id.ToString();
            user.LastModifiedDate = DateTime.Now;
            return await _context.SaveChangesAsync();
        }

        public async Task<int> EmployeeUserConnectedAsync()
        {
            try
            {
                var userNotConnected = await _context.Users.Where(i => i.IsActive == true && i.IsConnected == false).ToListAsync();

                if (userNotConnected.Count == 0)
                    return 0;

                var codes = userNotConnected.Select(i => i.Code).ToList();
                var employees = await _context.Employees.Where(i => i.IsActive && codes.Contains(i.Code)).ToListAsync();

                if (employees.Count == 0)
                    return 0;

                var corporate = await _context.Corporates.FirstOrDefaultAsync();
                var role = await _context.Roles.Where(i => i.IsDefault && i.IsActive).FirstOrDefaultAsync();

                foreach (var user in userNotConnected)
                {
                    user.Employee = employees.Where(i => i.Code == user.Code).FirstOrDefault();
                    user.Role = role;
                    user.Corporate = corporate;
                    user.IsConnected = true;
                    await _context.SaveChangesAsync();
                }
                return userNotConnected.Count;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, "");
                throw new Exception("TypeEmployee Create Error : " + ex.Message);
            }
        }

        public async Task<AspNetUser?> GetProfile()
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == userIdentity.Id.ToString())
                    .Include(r => r.Role)
                    .Include(c => c.Corporate)
                    .Include(e => e.Employee)
                    .FirstOrDefaultAsync();
                return user;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(""));
                throw new Exception("Bank GetProfile Error : " + ex.Message);
            }
        }

        public async Task<AspNetUser?> GetProfile(string code)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.Code == code)
                    .Include(r => r.Role)
                    .Include(c => c.Corporate)
                    .Include(e => e.Employee)
                    .FirstOrDefaultAsync();
                return user;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(code));
                throw new Exception("Bank GetProfile Error : " + ex.Message);
            }
        }

        public async Task<int> EmployeeUserConnectedAsync(string code)
        {
            try
            {
                var user = await _context.Users.Where(i => i.IsActive == true && i.Code == code)
                    .Include(i => i.Corporate)
                    .Include(i => i.Role)
                    .Include(i => i.Employee)
                    .FirstOrDefaultAsync();
                if (user == null)
                    return 0;

                if (user.Employee == null)
                {
                    var employees = await _context.Employees.Where(i => i.IsActive && i.Code == code).FirstOrDefaultAsync();
                    if (employees == null)
                        return 0;

                    user.Employee = employees;
                    user.IsConnected = true;
                }

                if (user.Role == null)
                {
                    var role = await _context.Roles.Where(i => i.IsDefault && i.IsActive).FirstOrDefaultAsync();
                    user.Role = role;
                }

                if (user.Corporate == null)
                {
                    var corporate = await _context.Corporates.FirstOrDefaultAsync();
                    user.Corporate = corporate;
                }
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, code);
                throw new Exception("TypeEmployee Create Error : " + ex.Message);
            }
        }
    }
}
