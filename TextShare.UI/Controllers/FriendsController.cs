using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.Users;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Контроллер для управления дружбой
    /// </summary>
    [Route("friends")]
    public class FriendsController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly UserManager<User> _userManager;

        public FriendsController(
            IFriendshipService friendshipService, 
            UserManager<User> userManager
            )
        {
            _friendshipService = friendshipService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> MyFriends(int page = 1)
        {
            User user = (await _userManager.GetUserAsync(User))!;
            List<Friendship> friends =  await _friendshipService.GetFriendshipsByUserIdAsync(user.Id);

            return View(friends);
        }

        [Authorize]
        [HttpGet("requests")]
        public async Task<IActionResult> FriendRequests(int page = 1)
        {

            return View();
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> FriendsSearch(int page = 1)
        {

            return View();
        }

    }
}
