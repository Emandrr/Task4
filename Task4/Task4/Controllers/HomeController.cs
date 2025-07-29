using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Task4.Models;
using Humanizer;
using Task4.Service;

namespace Task4.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly UserService _userService;
    public HomeController(ILogger<HomeController> logger, UserManager<User> userMngr, SignInManager<User> signInManager,UserService userService)
    {
        _logger = logger;
        _userManager = userMngr;
        _signInManager = signInManager;
        _userService = userService;
    }
    [Authorize]
    public IActionResult Index()
    {
        var users = _userManager.Users
            .OrderByDescending(u => u.LastLoginTime)
            .ToList();
        return View(users);
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Block(string[] UserId)
    {
        foreach (var id in UserId)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userService.BlockSelected(user); 
            }
        }
        return RedirectToAction("Index", "Home");
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Unblock(string[] UserId)
    {
        foreach (var id in UserId)
        {
            User user = await _userManager.FindByIdAsync(id);
            await _userService.UnBlockSelected(user);
        }
        return RedirectToAction("Index");
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(string[] UserId)
    {
        string id = _userManager.GetUserId(User);
        bool IsIDeleted = await _userService.DeleteAllSelected(UserId);
        return RedirectToAction("Index");
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
