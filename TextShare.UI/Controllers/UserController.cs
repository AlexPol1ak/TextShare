// Ignore Spelling: username

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
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

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
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
            User user = await _userManager.GetUserAsync(HttpContext.User);
            if(user == null) return NotFound();

            UserModel userModel = UserModel.FromUser(user);
            ResponseData<User> responseModel = new() { Data = user };
            return View(responseModel);
        }

        

    }
}
