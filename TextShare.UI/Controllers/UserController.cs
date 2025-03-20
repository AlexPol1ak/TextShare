// Ignore Spelling: username

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.FriendsModels;
using TextShare.Domain.Models.EntityModels.UserModels;
using TextShare.Domain.Settings;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Контроллер для управления пользователями
    /// </summary>
    [Route("profile")]
    public class UserController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IShelfService _shelfService;
        private readonly ITextFileService _textFileService;
        private readonly IUserService _userService;
        private readonly IFriendshipService _friendshipService;

        public UserController(
            UserManager<User> userManager,
            ITextFileService textFileService,
            IUserService userService,
            IShelfService shelfService,
            IFriendshipService friendshipService,
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadSettings
            

            ): base(physicalFile, imageUploadSettings)
        {
            _userManager = userManager;
            _textFileService = textFileService;
            _userService = userService;
            _shelfService = shelfService;
            _friendshipService = friendshipService;
        }

        [HttpGet("{username}")]
        [Authorize]
        public async Task<IActionResult> DetailsUserByUserName(string username)
        {
            return Content(username);
        }

        /// <summary>
        /// Страница пользователя
        /// </summary>
        /// <returns>Страница с личной информацией о пользователе</returns>
        /// <remarks>GET /profile/my </remarks>
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> DetailsUser()
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;
            UserModel userModel = UserModel.FromUser(currentUser);           
            return View(userModel);
        }

        
        [HttpGet("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser()
        {
            User user = (await _userManager.GetUserAsync(User))!;

            UserUpdateModel userUpdateModel = UserUpdateModel.FromUser(user);
            return View(userUpdateModel);
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UserUpdateModel userUpdateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userUpdateModel);
            }

            User user = (await _userManager.GetUserAsync(User))!;

            // Если пользователь хочет сменить пароль
            if (!string.IsNullOrEmpty(userUpdateModel.OldPassword) && !string.IsNullOrEmpty(userUpdateModel.NewPassword))
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, userUpdateModel.OldPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError("OldPassword", "Неверный текущий пароль.");
                    return View(userUpdateModel);
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, userUpdateModel.OldPassword, userUpdateModel.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(userUpdateModel);
                }
            }

            // Обновление данных пользователя
            user.FirstName = userUpdateModel.FirstName;
            user.LastName = userUpdateModel.LastName;
            user.Patronymic = userUpdateModel.Patronymic;
            user.SelfDescription = userUpdateModel.SelfDescription;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(userUpdateModel);
            }

            TempData["SuccessMessage"] = "Данные успешно обновлены!";
            return RedirectToAction("DetailsUser");
        }

        [HttpPost("update/upload-avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile? avatar)
        {
            User user = (await _userManager.GetUserAsync(User))!;
            UserUpdateModel userUpdateModel = UserUpdateModel.FromUser(user);

            if (avatar == null || avatar.Length == 0)
            {
                ViewData["AvatarError"] = "Файл не выбран!";
                return View("UpdateUser", userUpdateModel);
            }

            string? avatarUri = null;
            ResponseData<Dictionary<string, string>> data = await SaveImage(avatar);
            if (data.Success == false)
            {
                ViewData["AvatarError"] = data.ErrorMessage;
                View("UpdateUser", userUpdateModel);
            }

            avatarUri = data.Data.GetValueOrDefault("uri", null);

            if (!string.IsNullOrEmpty(avatarUri))
            {
                if(user.AvatarUri != null)
                {
                    await DeleteImageByUri(user.AvatarUri);

                }             
                user.AvatarUri = avatarUri;
                user.AvatarMimeType = avatar.ContentType;
                await _userManager.UpdateAsync(user);
            }

            // Возвращаем представление с обновлёнными данными
            userUpdateModel.AvatarUri = user.AvatarUri;
            return View("UpdateUser", userUpdateModel);

        }





    }
}
