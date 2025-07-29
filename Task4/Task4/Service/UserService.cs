using Microsoft.AspNetCore.Identity;

using Microsoft.Identity.Client;
using Task4.Contarcts.DTO;
using Task4.Models;
using Task4.ViewModels;

namespace Task4.Service
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public User SetUser(RegisterViewModel model)
        {
            return new User
            {
                Email = model.Email,
                UserName = model.Login+model.Email,
                Login=model.Login,
                RegistrationDate = DateTime.UtcNow,
                LastLoginTime = DateTime.UtcNow,
                IsEnable = true,
            };
            
        }
        public async Task<CreateUserResult> CreateUser(RegisterViewModel model)
        {
            User user = SetUser(model);
            var result = await _userManager.CreateAsync(user, model.Password);
            if(result.Succeeded) await _signInManager.SignInAsync(user, false);
            return new CreateUserResult{ User = user, Result = result };
        }
        public async Task UpdateUserTime(LoginViewModel model)
        {
            User user = await _userManager.FindByEmailAsync(model.Email);
            user.LastLoginTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }
        public async Task<SignInResult> TrySignIn(LoginViewModel model)
        {
            User user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return SignInResult.Failed;
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);
            return result;
        }
        public async Task BlockSelected(User user)
        {
            user.LockoutEnd = DateTime.UtcNow.AddYears(200);
            user.IsEnable = false;
            user.LockoutEnabled = true;
            var result = await _userManager.UpdateAsync(user);
        }
        public async Task UnBlockSelected(User user)
        {
            if (user != null)
            {
                user.LockoutEnd = DateTime.UtcNow;
                user.IsEnable = true;
                user.LockoutEnabled = false;
                var result = await _userManager.UpdateAsync(user);
            }
        }
        public async Task<bool> DeleteAllSelected(string[] UserId)
        {
            bool value = false;
            foreach (var id in UserId)
            {
                value = await DeleteSelectedNotSelf(id);
            }
            return value;
        }
        public async Task<bool> DeleteSelectedNotSelf(string SelectedId)
        {
            User user = await _userManager.FindByIdAsync(SelectedId);
            if (user != null)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                await _userManager.DeleteAsync(user);
                return false;
            }
            return true;
        }

    }
}
