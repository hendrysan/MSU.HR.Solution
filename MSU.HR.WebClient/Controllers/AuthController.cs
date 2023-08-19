using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Requests;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;
using Org.BouncyCastle.Asn1.Ocsp;
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
