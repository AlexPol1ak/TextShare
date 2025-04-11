using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.FriendsModels;
using TextShare.Domain.Utils;
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

        /// <summary>
        /// Отображение страницы "Мои друзья".
        /// </summary>
        /// <param name="page"></param>
        /// <returns>Страница с списком друзей</returns>
        /// <remarks>GET /friends?page=1 </remarks>
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> MyFriends(int page = 1)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;
            IEnumerable<User> friends = await _friendshipService.GetFriendsUser(currentUser.Id);

            IEnumerable<FriendshipSatusModel> friendModels = (await FriendshipSatusModel.FromUsers(friends))
                .Select(model =>
                {
                    model.FriendStatus = FriendStatus.Accepted;
                    return model;
                }).OrderBy(user => user.UserName);

            return View(friendModels.ToPagedList(page, 5));
        }

        /// <summary>
        /// Отображает страницу с друзьями пользователя
        /// </summary>
        /// <param name="username"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("{username}")]
        [Authorize]
        public async Task<IActionResult> UserFriends(string username, int page = 1)
        {
            // Просматриваемый поль-ль
            User? viewedUser = await _userService.GetUserByUsernameAsync(username, u => u.Friendships);
            if (viewedUser == null)
            {
                HttpContext.Items["ErrorMessage"] = $"Пользователь \"{username}\" не найден";
                return NotFound();
            }

            // Текущий поль-ль
            User currentUser = (await _userManager.GetUserAsync(User))!;
            List<int> currentUserFriends = (await _friendshipService.GetFriendsUser(currentUser.Id))
                .Select(u => u.Id).ToList();
            List<int> currentUserOutRequestFrineds = (await _friendshipService.GetOutFriendRequestsUsers(currentUser.Id))
                .Select(u => u.Id).ToList();
            List<int> currentUserInRequestFrineds = (await _friendshipService.GetInFriendRequestsUsers(currentUser.Id))
                .Select(u => u.Id).ToList();

            // Дружба авторизованного польз-ля и просматриваемого.
            Friendship? frViewedUser = (await _friendshipService.FindFriendshipsAsync(
                fr =>
                ((fr.UserId == currentUser.Id && fr.FriendId == viewedUser.Id)
                || (fr.UserId == viewedUser.Id && fr.FriendId == currentUser.Id))
                && fr.IsConfirmed == true
                )).FirstOrDefault();
            DebugHelper.ShowData(frViewedUser);
            if (frViewedUser == null && currentUser.Id != viewedUser.Id)
            {
                HttpContext.Items["ErrorMessage"] = $"Вы не можете просматривать эту страницу";
                return BadRequest();
            }

            // Друзья просматриваемого по-ля.
            List<User> friendsViewedUser = await _friendshipService.GetFriendsUser(viewedUser.Id, u => u.Friendships);
            List<FriendshipSatusModel> friendsModels = new();

            // Перебор друзей просматриваемого пользователя и формирование моделей.
            foreach (var friend in friendsViewedUser)
            {
                FriendshipSatusModel friendModel = await FriendshipSatusModel.FromUser(friend);

                int frindId = friend.Id;
                // Определение отношения текущего пользователя к другу просмтариваемого пользователя
                if (frindId == currentUser.Id)
                    friendModel.FriendStatus = FriendStatus.Iam;
                else if (currentUserFriends.Any(id => id == friend.Id))
                    friendModel.FriendStatus = FriendStatus.Accepted;
                else if (currentUserInRequestFrineds.Any(id => id == friend.Id))
                    friendModel.FriendStatus = FriendStatus.Pending;
                else if (currentUserOutRequestFrineds.Any(id => id == friend.Id))
                    friendModel.FriendStatus = FriendStatus.Requested;
                else
                    friendModel.FriendStatus = FriendStatus.None;

                friendsModels.Add(friendModel);

            }

            ViewData["viewedUsername"] = viewedUser.UserName;
            return View(friendsModels.ToPagedList(page, 5));

        }


        /// <summary>
        /// Отображение страницы входящих заявок в друзья.
        /// </summary>
        /// <param name="page"></param>
        /// <returns>Страница с списком входящих заявок в друзья</returns>
        /// <remarks>GET /friends/in-requests?page=1</remarks>
        [Authorize]
        [HttpGet("in-requests")]
        public async Task<IActionResult> FriendsInRequests(int page = 1)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;
            IEnumerable<User> inRequests = await _friendshipService.GetInFriendRequestsUsers(currentUser.Id);

            IEnumerable<FriendshipSatusModel> FriendModels = (await FriendshipSatusModel.FromUsers(inRequests))
                .Select(model =>
                {
                    model.FriendStatus = FriendStatus.Pending;
                    return model;
                });

            return View(FriendModels.ToPagedList(page, 5));
        }

        /// <summary>
        /// Отображает страницы исходящих заявок в друзья
        /// </summary>
        /// <param name="page"></param>
        /// <returns>Страница с списком исходящих заявок в друзья</returns>
        /// <remarks>GET /friends/out-requests?page=1</remarks>
        [Authorize]
        [HttpGet("out-requests")]
        public async Task<IActionResult> FriendsOutRequests(int page = 1)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;
            IEnumerable<User> outRequests = await _friendshipService.GetOutFriendRequestsUsers(currentUser.Id);

            IEnumerable<FriendshipSatusModel> FriendOutRequestsModels = (await FriendshipSatusModel.FromUsers(outRequests))
                .Select(model =>
                {
                    model.FriendStatus = FriendStatus.Requested;
                    return model;
                });
            return View(FriendOutRequestsModels.ToPagedList(page, 5));
        }

        /// <summary>
        /// Отображение страницы "Поиск"
        /// </summary>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> FriendsSearch(string? search = null, int page = 1)
        {
            // Получаем текущего пользователя
            User currentUser = (await _userManager.GetUserAsync(User))!;

            if (string.IsNullOrWhiteSpace(search))
            {
                return View((new List<FriendshipSatusModel>().ToPagedList()));
            }

            // Получаем друзей
            var friendships = await _friendshipService.FindFriendshipsAsync(
                f => f.UserId == currentUser.Id && f.IsConfirmed == true,
                f => f.Friend
                );
            var friends = friendships.Select(f => f.Friend).ToList();

            // Входящие заявки в друзья
            var inFriendRequests = (await _friendshipService.FindFriendshipsAsync(
                f => f.FriendId == currentUser.Id && !f.IsConfirmed, f => f.User))
                .Select(f => f.User)
                .ToList();

            // Исходящие заявки (текущий пользователь отправил запрос другим)
            var outFriendRequests = (await _friendshipService.FindFriendshipsAsync(
                f => f.UserId == currentUser.Id && !f.IsConfirmed, f => f.Friend))
                .Select(f => f.Friend)  // Тот, кому отправлен запрос
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
                var res2 = await _userService.FindUsersAsync(u =>
                    EF.Functions.Like(u.FirstName + " " + u.LastName, $"%{search}%"));

                resultSearch.AddRange(res1);
                resultSearch.AddRange(res2);
                resultSearch = resultSearch.DistinctBy(u => u.Id).ToList();
            }

            // Создаем HashSet для быстрого поиска статуса дружбы
            var friendIds = friends.Select(f => f.Id).ToHashSet();
            var inRequestIds = inFriendRequests.Select(f => f.Id).ToHashSet();
            var outRequestIds = outFriendRequests.Select(f => f.Id).ToHashSet();

            // Преобразуем пользователей в модели дружбы с правильными статусами
            List<FriendshipSatusModel> friendshipSatusModels = (await FriendshipSatusModel.FromUsers(resultSearch))
                .Select(model =>
                {
                    model.FriendStatus = model.Id == currentUser.Id ? FriendStatus.Iam :
                                         friendIds.Contains(model.Id) ? FriendStatus.Accepted :
                                         inRequestIds.Contains(model.Id) ? FriendStatus.Pending :
                                         outRequestIds.Contains(model.Id) ? FriendStatus.Requested :
                                         FriendStatus.None;
                    return model;
                })
                .OrderBy(model => model.FriendStatus)
                .ToList();

            return View(friendshipSatusModels.ToPagedList(page, 5));
        }


        /// <summary>
        /// Одобряет запрос дружбы
        /// </summary>
        /// <param name="username"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("accept-friend")]
        public async Task<IActionResult> AcceptFriendRequest(string username, string? returnUrl = null)
        {
            var result = await verifyUsername(username);
            if (result.Success == false || result.Data == null)
            {
                HttpContext.Items["ErrorMessage"] = result.ErrorMessage;
                return BadRequest();
            }
            User currentUser = (await _userManager.GetUserAsync(User))!;
            User requestedUser = result.Data;

            Friendship? friendship = (await _friendshipService.FindFriendshipsAsync(
                f => f.FriendId == currentUser.Id && f.UserId == requestedUser.Id && f.IsConfirmed == false
                )).FirstOrDefault();

            if (friendship == null)
            {
                HttpContext.Items["ErrorMessage"] = "Заявки не найдено";
                return BadRequest();
            }

            await _friendshipService.ConfirmFriendshipAsync(friendship.Id);
            await _friendshipService.SaveAsync();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("FriendsInRequests");
        }

        /// <summary>
        /// Добавление пользователя в друзья
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("add-friend")]
        public async Task<IActionResult> SendFriendRequest(string username, string? returnUrl = null)
        {
            var result = await verifyUsername(username);
            if (result.Success == false || result.Data == null)
            {
                HttpContext.Items["ErrorMessage"] = result.ErrorMessage;
                return BadRequest();
            }

            User user = (await _userManager.GetUserAsync(User))!;
            User requestedUser = result.Data;

            var fs = await _friendshipService.FindFriendshipsAsync(f =>
            (f.UserId == user.Id && f.FriendId == requestedUser.Id) ||
            (f.UserId == requestedUser.Id && f.FriendId == user.Id)
            );
            if (fs.Count < 1)
            {
                Friendship friendship = new()
                {
                    User = user,
                    Friend = requestedUser
                };
                await _friendshipService.CreateFriendshipAsync(friendship);
                await _friendshipService.SaveAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("MyFriends");
        }

        /// <summary>
        /// Удаление пользователя из друзей или удаление заявки
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("delete-friend")]
        public async Task<IActionResult> DeleteFriend(string username, string? returnUrl = null)
        {
            var result = await verifyUsername(username);
            if (result.Success == false || result.Data == null)
            {
                HttpContext.Items["ErrorMessage"] = result.ErrorMessage;
                return BadRequest();
            }

            User user = (await _userManager.GetUserAsync(User))!;
            User requestedUser = result.Data;

            var fs = await _friendshipService.FindFriendshipsAsync(f =>
           (f.UserId == user.Id && f.FriendId == requestedUser.Id) ||
           (f.UserId == requestedUser.Id && f.FriendId == user.Id)
           );

            if (fs.Count > 0)
            {
                Friendship friendship = fs.First();
                await _friendshipService.DeleteFriendshipAsync(friendship.Id);
                await _friendshipService.SaveAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("MyFriends");
        }

        /// <summary>
        /// Проверка существует ли пользователь по username.
        /// Проверяет существует ли пользователь,
        /// возвращает его если существует, если нет-возвращает ошибку.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private async Task<ResponseData<User?>> verifyUsername(string username)
        {
            ResponseData<User?> responseData = new();
            if (string.IsNullOrEmpty(username))
            {
                responseData.Success = false;
                responseData.ErrorMessage = "Пустой запрос";
                return responseData;
            }

            User? requestedUser = await _userService.GetUserByUsernameAsync(username);
            if (requestedUser == null)
            {
                responseData.Success = false;
                responseData.ErrorMessage = "Такого пользователя не существует.";
                return responseData;
            }

            responseData.Data = requestedUser;
            return responseData;

        }


    }
}
