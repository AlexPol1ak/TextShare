using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models.EntityModels.FriendsModels;
using X.PagedList.Extensions;

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
        private readonly IUserService _userService;

        public FriendsController(
            IFriendshipService friendshipService, 
            UserManager<User> userManager,
            IUserService userService
            )
        {
            _friendshipService = friendshipService;
            _userManager = userManager;
            _userService = userService;
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
        public async Task<IActionResult> FriendsSearch(string? search = null, int page = 1)
        {
            // Если строка поиска пустая, просто отобразить пустую страницу поиска
            var friendSearchResultModel = new FriendSearchResultModel();
            if (string.IsNullOrWhiteSpace(search))
            {
                return View(friendSearchResultModel);
            }

            User user = (await _userManager.GetUserAsync(User))!;

            // Получаем друзей и входящие заявки
            var friends = (await _friendshipService.GetFriendshipsByUserIdAsync(user.Id))
                .Select(f => f.User)
                .ToList();

            var friendRequests = (await _friendshipService.FindFriendshipsAsync(
                f => f.UserId == user.Id && !f.IsConfirmed))
                .Select(f => f.User)
                .ToList();

            // Поиск пользователей
            var resultSearch = new List<User>();

            User? exactUser = await _userService.GetUserByUsernameAsync(search);
            if (exactUser != null)
            {
                resultSearch.Add(exactUser);
            }
            else
            {
                var res1 = await _userService.FindUsersAsync(u => u.UserName!.Contains(search));
                var res2 = await _userService.FindUsersAsync(u =>EF.Functions.Like(u.FirstName + " " + u.LastName, $"%{search}%")
                                                );
                resultSearch.AddRange(res1);
                resultSearch.AddRange(res2);

                resultSearch = resultSearch.DistinctBy(u => u.Id).ToList();
            }

            var partResultSearch = resultSearch.ToPagedList(page, 5);

            // Передаем в модель данные
            friendSearchResultModel.Friends = friends;
            friendSearchResultModel.FriendRequests = friendRequests;
            friendSearchResultModel.ResultSearch = partResultSearch;
            return View(friendSearchResultModel);
        }

        [Authorize]
        public async Task<IActionResult> AcceptFriendRequest(int id)
        {
            return Content("");
        }

        [Authorize]
        public async Task<IActionResult> SendFriendRequest(int id)
        {
            User user = (await _userManager.GetUserAsync(User))!;
            User? requestedUser;

            return Content("");
        }

        [Authorize]
        public async Task<IActionResult> DeleteFriend(int id)
        {
            return Content("");
        }


    }
}
