using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models.EntityModels.UserModels;
using TextShare.Domain.Utils;
using TextShare.UI.ViewModels;

namespace TextShare.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Login()
        {
            await Task.CompletedTask;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Неверные учетные данные.");
            return View(model);
        }

        public async Task<IActionResult> Register()
        {
            await Task.CompletedTask;
            var model = new UserRegisterModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterModel model)
        {
            
            if (!ModelState.IsValid)
            {
                
                if (await _userManager.FindByNameAsync(model.UserName) != null)
                {
                    ModelState.AddModelError("UserName", "Этот имя пользователя уже занято. Попробуйте другое.");
                    return View(model); 
                }

                return View(model);
            }

            if(model.BirthDate > DateTime.Now.AddDays(3))
            {
                ModelState.AddModelError("BirthDate", "Не корректная дата рождения");
                return View(model);
            }
                         
            var user = model.ToUser();
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("DetailsUser", "User");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
