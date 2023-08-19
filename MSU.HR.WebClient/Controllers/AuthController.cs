using Microsoft.AspNetCore.Mvc;
using MSU.HR.Contexts;
using MSU.HR.Models.Requests;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;

namespace MSU.HR.WebClient.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            ViewData["returnUrl"] = returnUrl;
            var model = new LoginRequest();
            return View(model);            
        }

        [HttpPost]
        public IActionResult Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }

            

            return RedirectToAction("Index", "Home");
        }
    }
}
