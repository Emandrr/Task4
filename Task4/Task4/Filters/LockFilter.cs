using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Task4.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Azure;

namespace Task4.Filters
{
    public class LockFilter : IAsyncResourceFilter
    {
        private readonly string IdentityCookie = ".AspNetCore.Identity.Application";

        
        UserManager<User> _userManager;
        SignInManager<User> _signinManager;
        public LockFilter(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signinManager = signInManager;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context,ResourceExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var result = await _userManager.FindByIdAsync(_userManager.GetUserId(context.HttpContext.User));
                var currentUrl = context.HttpContext.Request.Path;
                if (result != null)
                {
                    if (result.IsEnable == false && currentUrl != "/Account/Login")
                    {
                        await _signinManager.SignOutAsync();
                        context.Result = new LocalRedirectResult("~/Account/Login");
                        return;
                    }
                }
                else
                {
                    await _signinManager.SignOutAsync();
                    context.Result = new LocalRedirectResult("~/Account/Login");
                    return;
                }
            }
            await next();
        }
        public void ClearCookie(ResourceExecutingContext context)
        {
            foreach (var cookie in context.HttpContext.Request.Cookies.Keys)
            {
                if(cookie==IdentityCookie)context.HttpContext.Response.Cookies.Delete(cookie);
            }
        }

    }
}
