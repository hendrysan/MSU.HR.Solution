using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Requests;
using MSU.HR.Services.Interfaces;
using System.Security.Claims;

namespace MSU.HR.WebClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUser _user;
        private readonly IEmployee _employee;
        private readonly IToken _token;
        private readonly UserManager<AspNetUser> _userManager;

        public AuthController(IUser user, UserManager<AspNetUser> userManager, IEmployee employee, IToken token)
        {
            _user = user;
            _userManager = userManager;
            _employee = employee;
            _token = token;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            ViewData["returnUrl"] = returnUrl;
            var model = new LoginRequest();
            model.CodeNIK = "123456789";
            model.Password = "123456789";
            return View(model);
        }


        [HttpGet]
        public IActionResult Registration()
        {
            RegisterRequest model = new RegisterRequest();
            return View(model);
        }

        [HttpPost("RegisterAsync")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                request.Password = "";
                request.ConfirmPassword = "";
                ModelState.AddModelError("ModelState", "Invalid login attempt");
                return View(request);
            }

            if (request.Password != request.ConfirmPassword)
            {
                request.Password = "";
                request.ConfirmPassword = "";
                ModelState.AddModelError("ConfirmPassword", "Password not Match");
                return View(request);
            }

            var user = await _user.GetProfile(code: request.CodeNIK);
            if (user is null)
            {
                request.Password = "";
                request.ConfirmPassword = "";
                ModelState.AddModelError("CodeNIK", "Code Not Found");
                return View(request);
            }

            var managedUser = await _userManager.FindByEmailAsync(email: user.Email);
            if (managedUser is not null)
            {
                request.Password = "";
                request.ConfirmPassword = "";
                ModelState.AddModelError("Email", "Email Already Registered");
                return View(request);
            }

            var employee = await _employee.GetEmployeeAsync(code: request.CodeNIK);
            if (employee is null)
            {
                var task = await _user.EmployeeUserConnectedAsync(user.Code);
                if (task == 0)
                    throw new Exception(message: "User to Employee failed Synchronized process, Please Call Administrator");
            }

            var userNew = new AspNetUser()
            {
                FullName = request.FullName ?? string.Empty,
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
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return View(request);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("ModelState", "Invalid login attempt");
                return View(request);
            }

            var user = await _user.GetProfile(code: request.CodeNIK);
            if (user is null)
            {
                ModelState.AddModelError("CodeNIK", "Code Not Found");
                return View(request);
            }

            var managedUser = await _userManager.FindByEmailAsync(email: user.Email);
            if (managedUser is null)
            {
                ModelState.AddModelError("CodeNIK", "User Undefine");
                return View(request);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);
            if (!isPasswordValid)
            {
                ModelState.AddModelError("Password", "Password not Match");
                return View(request);
            }

            var employee = await _employee.GetEmployeeAsync(code: request.CodeNIK);
            if (employee is null)
            {
                var task = await _user.EmployeeUserConnectedAsync(user.Code);
                if (task == 0)
                    throw new Exception(message: "User to Employee failed Synchronized process, Please Call Administrator");
            }

            var claims = _token.CreateClaims(user, user.Corporate, user.Role, employee);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);


            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            string? returnUrl = HttpContext.Request.Query["returnUrl"];

            HttpContext.Session.SetString("Alert", "");
            return Redirect(returnUrl ?? "/");

        }

        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
