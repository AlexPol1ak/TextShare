// Ignore Spelling: username

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.FriendsModels;
using TextShare.Domain.Models.EntityModels.UserModels;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Контроллер для управления пользователями
    /// </summary>
    [Route("profile")]
    public class UserController : Controller
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
            IFriendshipService friendshipService

            )
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

        

    }
}
