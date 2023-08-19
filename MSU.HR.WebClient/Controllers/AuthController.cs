using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSU.HR.Contexts;
using MSU.HR.Models.Requests;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;

namespace MSU.HR.WebClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly DatabaseContext _context;

        public AuthController(DatabaseContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            ViewData["returnUrl"] = returnUrl;
            var model = new LoginRequest();
            return View(model);            
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }

            var user = await _context.Users.Where(i=>i.Code== model.CodeNIK).FirstOrDefaultAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
