using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task4.Models;
using Task4.Service;
using Task4.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Task4.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly UserService _userService;

        private readonly string EmailProblem = "DuplicateEmail";

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,UserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var answer = await _userService.CreateUser(model);
                if (answer.Result.Succeeded) return RedirectToAction("Index", "Home");
                foreach (var error in answer.Result.Errors)
                {
                    if(error.Code==EmailProblem)ModelState.AddModelError(string.Empty, "This is email is already taken");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.TrySignIn(model);
                if (result.Succeeded)
                {
                    await _userService.UpdateUserTime(model);
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl);                  
                    return RedirectToAction("Index", "Home");  
                }
                if (result.IsLockedOut) ModelState.AddModelError(string.Empty,"You are blocked");
                else if(!result.Succeeded) ModelState.AddModelError(string.Empty, "Invalid login and (or) password");
            }
            return View(model);
        }
    }
}
