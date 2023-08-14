using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Models.Requests;
using MSU.HR.Models.Responses;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;

namespace MSU.HR.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IToken _token;
        private readonly ILogError _logError;
        private readonly IUser _user;
        private readonly IEmployee _employee;
        //private readonly DatabaseContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string loginProvider = "bearer";

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration, UserManager<AspNetUser> userManager, IToken token, ILogError logError, IUser user, IEmployee employee)
        {
            _token = token;
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            //_context = context;
            _logError = logError;
            _user = user;
            _employee = employee;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           

            var user = await _user.GetProfile(code: request.CodeNIK);
            if (user is null)
                return Unauthorized();

            var managedUser = await _userManager.FindByEmailAsync(email: user.Email);
            if (managedUser == null)
                return BadRequest("Bad credentials");

            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
            if (!isPasswordValid)
                return BadRequest("Bad credentials");


            var employee = await _employee.GetEmployeeAsync(code: request.CodeNIK);
            //if (employee is null)
            //    return Unauthorized();


            //List<Claim>? newClaims = null;
            string accessToken;

            accessToken = _token.CreateToken(user, user.Corporate, user.Role, employee);

            var response = new LoginResponse()
            {
                Token = accessToken,
                Code = user.Code,
                Email = user.Email,
                FullName = user.FullName,
                RefreshToken = Guid.NewGuid().ToString(),
                Role = user.Role?.Id.ToString(),
                UserId = user.Id,
                Username = user.UserName
            };

            return Ok(response);
        }

        [HttpGet("Profile")]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var identity = new UserIdentityModel(User.Identity as ClaimsIdentity);
            var user = await _user.GetProfile();//_context.Users.Include(r => r.Role).Include(c => c.Corporate).Include(e => e.Employee).FirstOrDefault(u => u.Id == identity.Id.ToString());
            if (user is null) return Unauthorized("silahkan login");

            return Ok(user);
        }

        [HttpGet("EmployeeConnection")]
        [Authorize]
        public async Task<IActionResult> EmployeeConnection()
        {
            var task = await _user.EmployeeUserConnectedAsync();
            return Ok(task);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AspNetUser()
            {
                FullName = request.FullName,
                UserName = request.Username,
                Email = request.Email,
                Code = request.CodeNIK,
                IsActive = true,
                CreatedDate = DateTime.Now,
                CreatedBy = Guid.Empty.ToString(),
                LastModifiedBy = Guid.Empty.ToString(),
                LastModifiedDate = DateTime.Now

            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _user.EmployeeUserConnectedAsync(code: request.CodeNIK);
                request.Password = "";
                return CreatedAtAction(nameof(Register), new { email = request.Email }, request);
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

    }
}
