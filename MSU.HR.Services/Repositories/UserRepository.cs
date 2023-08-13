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


        public async Task<int> ChangePassword()
        {
            //var users = await _context.Users.Where(i => i.IsActive == true && i.Employee == null).ToList();
            throw new NotImplementedException();
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

                var corporate = await _context.Corporates.FirstAsync();
                var role = await _context.Roles.FirstAsync();

                foreach (var user in userNotConnected)
                {
                    user.Employee = employees.Where(i => i.Code == user.Code).First();
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
                var user = await _context.Users.Where(u => u.Id == userIdentity.Id.ToString()).Include(r => r.Role).Include(c => c.Corporate).Include(e => e.Employee).FirstOrDefaultAsync();
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
                var user = await _context.Users.Where(u => u.Code == code).Include(r => r.Role).Include(c => c.Corporate).Include(e => e.Employee).FirstOrDefaultAsync();
                return user;
            }
            catch (Exception ex)
            {
                await _logError.SaveAsync(ex, JsonSerializer.Serialize(code));
                throw new Exception("Bank GetProfile Error : " + ex.Message);
            }
        }
    }
}
