using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.UserModels;
using TextShare.Domain.Settings;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Контроллер для управления учетными записями пользователей.
    /// </summary>
    /// <remarks>
    /// Маршрут по умолчанию: <c>/Account/{action}</c>
    /// </remarks>
    public class AccountController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IShelfService _shelfService;
        private readonly IAccessRuleService _accessRuleService;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IShelfService shelfService,
            IAccessRuleService accessRuleService,
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadOptions
            ) : base(physicalFile, imageUploadOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _shelfService = shelfService;
            _accessRuleService = accessRuleService;
        }

        /// <summary>
        /// Отображает страницу входа.
        /// </summary>
        /// <returns>Страница входа.</returns>
        /// <remarks>GET /Account/Login</remarks>
        public async Task<IActionResult> Login()
        {
            await Task.CompletedTask;
            return View();
        }

        /// <summary>
        /// Обрабатывает POST-запрос для входа.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// Если успешно перенаправляет на страницу пользователя,
        /// иначе возвращает страницу с ошибками.
        /// </returns>
        /// <remarks>POST /Account/Login</remarks>
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                bool passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!passwordValid)
                {
                    ModelState.AddModelError("Password", "Неверный пароль!");
                    return View(model);
                }
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError("Email", "Email не подтвержден!");
                    return View(model);

                }

                // Вход
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                    return RedirectToAction("DetailsUser", "User");
            }

            ModelState.AddModelError("", "Неверные учетные данные.");
            return View(model);
        }

        /// <summary>
        /// Отображает страницу регистрации.
        /// </summary>
        /// <returns>Страница регистрации</returns>
        /// <remarks>GET /Account/Register</remarks>
        public async Task<IActionResult> Register()
        {
            await Task.CompletedTask;
            var model = new UserRegisterModel();
            return View(model);
        }

        /// <summary>
        /// Обрабатывает POST-запрос для регистрации
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// Если успешно перенаправляет на страницу пользователя,
        /// иначе возвращает страницу с ошибками.
        /// </returns>
        /// <remarks>POST /Account/Register</remarks>
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterModel model, IFormFile? AvatarFile)
        {

            if (!ModelState.IsValid)
            {

                return View(model);
            }

            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "Этот имя пользователя уже занято. Попробуйте другое.");
                return View(model);
            }

            if (model.BirthDate > DateTime.Now.AddDays(3))
            {
                ModelState.AddModelError("BirthDate", "Не корректная дата рождения");
                return View(model);
            }

            string? avatarUri = null;
            if (AvatarFile != null)
            {
                ResponseData<Dictionary<string, string>> data = await SaveImage(AvatarFile);
                if (data.Success == false)
                {
                    ViewData["AvatarError"] = data.ErrorMessage;
                    return View(model);
                }
                avatarUri = data?.Data.GetValueOrDefault("uri", null);
            }
            var user = model.ToUser();
            user.EmailConfirmed = true; // Подтверждение Email
            if (avatarUri != null) user.AvatarUri = avatarUri;
            var result = await _userManager.CreateAsync(user, model.Password);

            // Добавление базовой полки.
            AccessRule shelfAccessRule = new(); //Правило доступа для полки
            await _accessRuleService.CreateAccessRuleAsync(shelfAccessRule);
            await _accessRuleService.SaveAsync();
            Shelf baseShelf = new Shelf()
            {
                Creator = user,
                Name = $"{user.UserName}_1",
                Description = "Моя первая полка",
                CanDeleted = false,
                AccessRule = shelfAccessRule,
                AccessRuleId = shelfAccessRule.AccessRuleId
            };

            await _shelfService.CreateShelfAsync(baseShelf);
            await _shelfService.SaveAsync();

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("DetailsUser", "User");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        /// <summary>
        /// Выход пользователя из системы
        /// </summary>
        /// <returns>Перенаправляет на домашнюю страницу</returns>
        /// <remarks>GET /Account/Logout</remarks>
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
