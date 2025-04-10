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
        private readonly IAccessСontrolService _accessControlService;

        public UserController(
            UserManager<User> userManager,
            ITextFileService textFileService,
            IUserService userService,
            IShelfService shelfService,
            IFriendshipService friendshipService,
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadSettings,
            IAccessСontrolService accessСontrolService
            

            ): base(physicalFile, imageUploadSettings)
        {
            _userManager = userManager;
            _textFileService = textFileService;
            _userService = userService;
            _shelfService = shelfService;
            _friendshipService = friendshipService;
            _accessControlService = accessСontrolService;
        }

        [HttpGet("{username}")]
        [Authorize]
        public async Task<IActionResult> DetailsUserByUserName(string username)
        {
            User? viewedUser = await _userService.GetUserByUsernameAsync(username);
            if(viewedUser == null)
            {
                HttpContext.Items["ErrorMessage"] = $"Пользователь \"{username}\" не найден";
                return NotFound();
            }
            UserDetailModel modelViewedUser = UserDetailModel.FromUser(viewedUser);

            User currentUser = (await _userManager.GetUserAsync(User))!;
            //Если пользователь просматривает сам себя
            if (currentUser.Id == viewedUser.Id)
                modelViewedUser.RelationshipTocurrentUser = FriendStatus.Iam;
            else
            {
                // Проверяем статус дружбы между просматриваемым пользователем и текущим пользователем
                var friendships = await _friendshipService.FindFriendshipsAsync(
                    f =>
                    (f.UserId == currentUser.Id && f.FriendId == viewedUser.Id) ||
                    (f.UserId == viewedUser.Id && f.FriendId == currentUser.Id)
                    );
                if (friendships != null && friendships.Count() > 0)
                {
                    Friendship friendship = friendships[0];
                    if (friendship.IsConfirmed)
                        modelViewedUser.RelationshipTocurrentUser = FriendStatus.Accepted;
                    else if (friendship.UserId == currentUser.Id && friendship.FriendId == viewedUser.Id)
                        modelViewedUser.RelationshipTocurrentUser = FriendStatus.Requested;
                    else if (friendship.UserId == viewedUser.Id && friendship.FriendId == currentUser.Id)
                        modelViewedUser.RelationshipTocurrentUser = FriendStatus.Pending;

                }
                else
                    modelViewedUser.RelationshipTocurrentUser = FriendStatus.None;
            }

            int countViewedUserFriends = (await _friendshipService.GetFriendsUser(viewedUser.Id)).Count();
            int countViewedUserAvvShelf = (await _shelfService.FindShelvesAsync(
                s=> s.CreatorId == viewedUser.Id && s.AccessRule.AvailableAll == true
                )
                ).Count();
            int countViewedUserAvvTextFiles = ( await _textFileService.FindTextFilesAsync(
                t=>t.OwnerId == viewedUser.Id && t.AccessRule.AvailableAll == true
                )
                ).Count();

            modelViewedUser.CountAvailableShelves = countViewedUserAvvShelf;
            modelViewedUser.CountAvailableTextFiles = countViewedUserAvvTextFiles;
            modelViewedUser.CountFriends = countViewedUserFriends;
                
            return View(modelViewedUser);
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
            //UserModel userModel = UserModel.FromUser(currentUser);           
            //return View(userModel);
            return RedirectToAction("DetailsUserByUserName", new { username = currentUser.UserName});
        }

        /// <summary>
        /// Отображает страницу обновления информации пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser()
        {
            User user = (await _userManager.GetUserAsync(User))!;

            UserUpdateModel userUpdateModel = UserUpdateModel.FromUser(user);
            return View(userUpdateModel);
        }

        /// <summary>
        /// Обрабатывает POST запрос обновления информации пользователя.
        /// </summary>
        /// <param name="userUpdateModel"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Обрабатывает POST запрос обновления аватара пользователя
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
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
