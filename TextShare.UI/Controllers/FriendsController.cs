using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
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
        /// <returns></returns>
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> MyFriends(int page = 1)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;
            IEnumerable<User> friends = (await _friendshipService.GetAllUserAcceptedFriendshipAsync(
                        currentUser.Id, f => f.User, f => f.Friend))
                        .SelectMany(f => new List<User> { f.User, f.Friend })
                        .Distinct()
                        .Where(user=> user.Id != currentUser.Id);

            IEnumerable<FriendshipSatusModel> friendModels = (await FriendshipSatusModel.FromUsers(friends))
                .Select(model => {
                    model.FriendStatus = FriendStatus.Accepted;
                    return model;
                }).OrderBy(user => user.UserName);

            return View(friendModels.ToPagedList(page, 5));
        }

        /// <summary>
        /// Отображение страницы входящих заявок в друзья.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("requests")]
        public async Task<IActionResult> FriendRequests(int page = 1)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;
            IEnumerable<User> inRequests = (await _friendshipService.FindFriendshipsAsync(
                f => f.FriendId == currentUser.Id && f.IsConfirmed == false, f => f.User
                )).Select(u => u.User);

            IEnumerable<FriendshipSatusModel> FriendModels = (await FriendshipSatusModel.FromUsers(inRequests))
                .Select(model=> { 
                    model.FriendStatus = FriendStatus.Pending; 
                    return model; 
                });

            return View(FriendModels.ToPagedList(page, 5));
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
                f => f.FriendId == currentUser.Id && !f.IsConfirmed, f=>f.User))  
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
                    model.FriendStatus = model.Id == currentUser.Id? FriendStatus.Iam:
                                         friendIds.Contains(model.Id) ? FriendStatus.Accepted :
                                         inRequestIds.Contains(model.Id) ? FriendStatus.Pending :
                                         outRequestIds.Contains(model.Id) ? FriendStatus.Requested :
                                         FriendStatus.None;
                    return model;
                })
                .OrderBy(model=>model.FriendStatus)
                .ToList();

            return View(friendshipSatusModels.ToPagedList(page, 5));
        }


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

            Friendship? friendship =( await _friendshipService.FindFriendshipsAsync(
                f => f.FriendId == currentUser.Id && f.UserId == requestedUser.Id && f.IsConfirmed == false
                )).FirstOrDefault();
            
            if(friendship == null)
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

            return RedirectToAction("FriendRequests");
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
            if(result.Success == false || result.Data == null)
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
            if(fs.Count < 1)
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

            if(fs.Count > 0)
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
            if(requestedUser == null)
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
