using Azure.Core;
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
using Org.BouncyCastle.Asn1.Ocsp;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MSU.HR.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly IToken _token;
        private readonly IUser _user;
        private readonly IEmployee _employee;
        //private readonly DatabaseContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string loginProvider = "bearer";

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration, UserManager<AspNetUser> userManager, IToken token, IUser user, IEmployee employee)
        {
            _token = token;
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            //_context = context;
            _user = user;
            _employee = employee;
        }

        [HttpPost]
        [Route("TestError")]
        public async Task<IActionResult> TestError([FromBody] BankRequest request)
        {
            //var identity = new UserIdentityModel(User.Identity as ClaimsIdentity);
            //var user = await _user.GetProfile();//_context.Users.Include(r => r.Role).Include(c => c.Corporate).Include(e => e.Employee).FirstOrDefault(u => u.Id == identity.Id.ToString());
            //if (user is null) return Unauthorized("silahkan login");

            //return Ok(user);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Unauthorized();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _user.GetProfile(code: request.CodeNIK);
            if (user is null)
                return Unauthorized(new ErrorModel()
                {
                    IsSuccess = false,
                    ErrorCode = 401,
                    Message = "Code not found"
                });

            var managedUser = await _userManager.FindByEmailAsync(email: user.Email);
            if (managedUser == null)
                return BadRequest(new ErrorModel()
                {
                    ErrorCode = 400,
                    IsSuccess = false,
                    Message = "Check email user is Empty"

                });

            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
            if (!isPasswordValid)
                return BadRequest(new ErrorModel()
                {
                    ErrorCode = 400,
                    IsSuccess = false,
                    Message = "Password is wrong"

                });

            var employee = await _employee.GetEmployeeAsync(code: request.CodeNIK);
            if (employee is null)
            {
                var task = await _user.EmployeeUserConnectedAsync(user.Code);
                if (task == 0)
                    throw new Exception(message: "User to Employee failed Synchronized process, Please Call Administrator");
            }

            user.RefreshToken = _token.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = _token.GetRefreshTokenExpiryTime();


            string accessToken;
            accessToken = _token.CreateToken(user, user.Corporate, user.Role, employee, user.RefreshTokenExpiryTime.Value);
            await _userManager.UpdateAsync(user);

            var response = new LoginResponse()
            {
                Token = accessToken,
                Code = user.Code,
                Email = user.Email,
                FullName = user.FullName,
                RefreshToken = user.RefreshToken,//Guid.NewGuid().ToString(),
                Role = user.Role?.Id.ToString(),
                UserId = user.Id,
                Username = user.UserName,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime.Value,
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken(LoginResponse login)
        {
            if (login is null)
            {
                return BadRequest(new ErrorModel()
                {
                    IsSuccess = false,
                    Message = "Invalid client request",
                    ErrorCode = 400

                });
            }

            var principal = _token.GetPrincipalFromExpiredToken(login.Token);
            if (principal == null)
            {
                //return BadRequest("Invalid access token or refresh token");
                return BadRequest(new ErrorModel()
                {
                    ErrorCode = 400,
                    IsSuccess = false,
                    Message = "Invalid access token or refresh token"
                });
            }

            var user = await _user.GetProfile(code: login.Code);
            if (user == null || user.RefreshToken != login.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest(new ErrorModel()
                {
                    ErrorCode = 400,
                    IsSuccess = false,
                    Message = "Invalid access token or refresh token"
                });
            }

            user.RefreshToken = _token.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = _token.GetRefreshTokenExpiryTime();


            string newAccessToken;
            newAccessToken = _token.CreateToken(user, user.Corporate, user.Role, user.Employee, user.RefreshTokenExpiryTime.Value);
            await _userManager.UpdateAsync(user);


            var response = new LoginResponse()
            {
                Token = newAccessToken,
                Code = login.Code,
                Email = login.Email,
                FullName = login.FullName,
                RefreshToken = user.RefreshToken,//Guid.NewGuid().ToString(),
                Role = login.Role,
                UserId = login.UserId,
                Username = login.Username,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime.Value,
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
