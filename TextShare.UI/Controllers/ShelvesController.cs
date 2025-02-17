using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Net;
using System.Runtime.CompilerServices;
using TextShare.Business.Interfaces;
using TextShare.Business.Services;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.ShelfModels;
using TextShare.Domain.Settings;
using TextShare.Domain.Utils;
using X.PagedList;
using X.PagedList.Extensions;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Котроллер для управления полками.
    /// </summary>
    [Route("shelves")]
    public class ShelvesController : Controller
    {
        private readonly IShelfService _shelfService;
        private readonly IAccessRuleService _accessRuleService;       
        private readonly IUserService _userService;
        private readonly IFriendshipService _friendshipService;
        private readonly IGroupService _groupService;
        private readonly ShelvesSettings _shelvesSettings;
        private readonly UserManager<User> _userManager;
        


        public ShelvesController(IShelfService shelfService,
            UserManager<User> userManager,
            IAccessRuleService accessRuleService,
            IUserService userService,
            IOptions<ShelvesSettings> shelvesSettingsOptions,
            IFriendshipService friendshipService,
            IGroupService groupService
            )
        {
            _shelfService = shelfService;
            _userManager = userManager;
            _accessRuleService = accessRuleService; 
            _userService = userService;
            _shelvesSettings = shelvesSettingsOptions.Value;
            _friendshipService = friendshipService;
            _groupService = groupService;

            
        }

        /// <summary>
        /// Отображает страницу с полками пользователя.
        /// </summary>
        /// <param name="page">Страница полок</param>
        /// <returns></returns>
        /// <remarks>shelves/my?page=1</remarks>
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> MyShelves(int page = 1)
        {

            int pageSize = _shelvesSettings.MaxNumberShelvesInPage;
            User user = (await _userManager.GetUserAsync(User))!;           
            List<Shelf> userShelvesAll = await _shelfService.GetAllUserShelvesAsync(user.Id);
            
            IPagedList<Shelf> shelvesPart = userShelvesAll.ToPagedList(page, pageSize);

            AvailableShelvesModel<User, IPagedList<Shelf>> responseModel = new();
            responseModel.User = user;
            responseModel.AvailableShelves = shelvesPart;

            return View(responseModel);
        }

        /// <summary>
        /// Отображает страницу полок, к которым предоставили  доступ друзья.
        /// </summary>
        /// <returns></returns>
        /// <remarks>shelves/friends-shared?page=1</remarks>
        [Authorize]
        [HttpGet("friends-shared")]
        public async Task<IActionResult> AvailableFromFriends(int page=1)
        {

            User user = (await _userManager.GetUserAsync(User))!;
            int pageSize = _shelvesSettings.MaxNumberShelvesInPage;
            AvailableShelvesModel<User, IPagedList<Shelf>> responseModel = new();
            responseModel.User = user;

            // Получаем список друзей
            List <Friendship> userFriendships = await _friendshipService.GetFriendshipsByUserIdAsync(user.Id);

            if (userFriendships.Count == 0)
            {
                responseModel.AvailableShelves = new List<Shelf>().ToPagedList(page, pageSize);
                return View(responseModel);
            }

            // Доступные полки пользователю
            List<Shelf> availableShelves = new();

            foreach (var friendship in userFriendships)
            {
                // Получаем полки друга вместе с правилами доступа и пользователями,кому эта полка доступна.
                List<Shelf> friendShelves = await _shelfService.GetAllUserShelvesAsync(
                    friendship.FriendId,
                    s => s.AccessRule.AvailableUsers
                );
                // Фильтруем только те полки, где текущий пользователь есть в списке доступных
                availableShelves.AddRange(friendShelves
                    .Where(s => s.AccessRule.AvailableUsers.Any(u => u.Id == user.Id)));
            }
            // Преобразуем в пагинированный список
            IPagedList<Shelf> pagedShelves = availableShelves.ToPagedList(page, 10);
            responseModel.AvailableShelves = pagedShelves;

            return View(responseModel);
        }

        /// <summary>
        /// Отображает страницу полок, которые доступны группам, в которых состоит пользователь.
        /// </summary>
        /// <returns></returns>
        /// <remarks>shelves/shared-from-groups?page=1</remarks>
        [Authorize]
        [HttpGet("shared-from-groups")]
        public async Task<IActionResult> AvailableFromGroups(int page=1)
        {
            int pageSize = _shelvesSettings.MaxNumberShelvesInPage;

            User user = (await _userManager.GetUserAsync(User))!;
            User userDb = (await _userService.GetUserByIdAsync(user.Id, u=>u.GroupMemberships))!;

            // Группы ,в которых состоит пользователь.
            List<Group> userGroups = await _groupService.FindGroupsAsync(
                g => g.Members.Any(m => m.UserId == user.Id && m.IsConfirmed == true),
                g => g.AccessRules);

            // Полки, доступные группам, в которых состоит пользователь
            List<Shelf> availableShelves = userGroups
                .SelectMany(g => g.AccessRules)
                .Where(ar => ar.Shelf != null)
                .Select(ar => ar.Shelf)
                .Distinct()
                .ToList();

            AvailableShelvesModel<User, IPagedList<Shelf>> responseModel = new();
            responseModel.User = userDb;
            responseModel.AvailableShelves = availableShelves.ToPagedList(page, pageSize);
          
            return View(responseModel);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] ShelvesSearchModel? shelvesSearchModel = null, int page= 1)
        {
            
            if (shelvesSearchModel == null || 
                (string.IsNullOrEmpty(shelvesSearchModel.Name) && string.IsNullOrEmpty(shelvesSearchModel.Description)))
            {
                return View("SearchInput");
            }

            if (!ModelState.IsValid)
            {
                return View("SearchInput", shelvesSearchModel);
            }
            int pageSize = _shelvesSettings.MaxNumberShelvesInPage;
            List<Shelf> shelvesResult = new(); // Результат всего поиска
            List<Shelf> shelvesResponse = new(); // Результат фильтрации и ответ пользователю
            string? name = shelvesSearchModel.Name;
            string? description = shelvesSearchModel.Description;
            bool onlyAvailableMe = shelvesSearchModel.OnlyAvailableMe;

            // Если указано имя и описание
            if ((!string.IsNullOrEmpty(name)) && (!string.IsNullOrEmpty(description)))
            {
                shelvesResult = await _shelfService.FindShelvesAsync(
                    s => s.Name.Contains(name) && s.Description.Contains(description),
                    s=>s.AccessRule.AvailableGroups,
                    s=>s.AccessRule.AvailableUsers
                    );
            }

            // Если указано только имя 
            if ((!string.IsNullOrEmpty(name)) && (string.IsNullOrEmpty(description)))
            {
                shelvesResult = await _shelfService.FindShelvesAsync(
                   s => s.Name.Contains(name),
                   s => s.AccessRule.AvailableGroups,
                   s => s.AccessRule.AvailableUsers
                   );
            }

            // Если указано только описание
            if ((string.IsNullOrEmpty(name)) && (!string.IsNullOrEmpty(description)))
            {
                shelvesResult = await _shelfService.FindShelvesAsync(
                    s => s.Description.Contains(description),
                    s => s.AccessRule.AvailableGroups,
                    s => s.AccessRule.AvailableUsers
                    );
            }

            // Если пользователь не авторизован или авторизован но поиск указан по общедоступным полкам
            // Фильтруем только общедоступные полки
            if ((!User.Identity.IsAuthenticated ) || (User.Identity.IsAuthenticated && (onlyAvailableMe == false)))
            {
                shelvesResponse = shelvesResult.Where(s => s.AccessRule.AvailableAll == true).ToList();
                
            }

            // Если пользователь авторизован и указан поиск по доступным ему полкам.
            if (User.Identity.IsAuthenticated && onlyAvailableMe == true)
            {
                User user = (await _userManager.GetUserAsync(User))!;
                User userDb = (await _userService.GetUserByIdAsync(user.Id, u => u.GroupMemberships))!;
                List<int> userGroupIds = userDb.GroupMemberships.Select(g => g.GroupId).ToList();

                shelvesResponse = shelvesResult
                    .Where(s => s.AccessRule.AvailableUsers.Any(u => u.Id == userDb.Id) ||
                                s.AccessRule.AvailableGroups.Any(group => userGroupIds.Contains(group.GroupId)))
                    .DistinctBy(s=>s.ShelfId)
                    .ToList();
            }

            return View("SearchOutput", shelvesResponse.ToPagedList(page, pageSize));
        }

        [Authorize]
        [HttpGet("create-shelf")]
        public async Task<IActionResult> CreateShelf()
        {
            return Content("Create Shelf Get");
        }

        [Authorize]
        [HttpPost("create-shelf")]
        public async Task<IActionResult> CreateShelf(ShelfDetailModel shelfModel)
        {
            return Content("Create Shelf Post");
        }


        [HttpGet("detail/{id}")]
        public async Task<IActionResult> DetailShelf(int id)
        {

            Shelf? shelf = await _shelfService.GetShelfByIdAsync(id,
                s => s.Creator,
                s => s.TextFiles,
                s => s.AccessRule.AvailableGroups,
                s=>s.AccessRule.AvailableUsers);
            
            // Если полка не найдена
            if (shelf == null) return NotFound();
            ShelfDetailModel shelfDetailModel = ShelfDetailModel.FromShelf(shelf);

            // Если доступна всем
            if (shelf.AccessRule.AvailableAll == true) return View(shelfDetailModel);

            if (!User.Identity.IsAuthenticated) return Challenge();

            User userDb = await _userService.GetUserByIdAsync((await _userManager.GetUserAsync(User)).Id,
                u=>u.GroupMemberships);

            if(shelf.CreatorId == userDb.Id) return View(shelfDetailModel);
            if(shelf.AccessRule.AvailableUsers.Any(u=>u.Id == userDb.Id)) return View(shelfDetailModel);

            var ids = new HashSet<int>((shelf.AccessRule.AvailableGroups.Select(g => g.GroupId)));
            bool hasIntersection = userDb.GroupMemberships.Any(g => ids.Contains(g.GroupId));
            if (hasIntersection) return View(shelfDetailModel);

            return RedirectToAction("Login", "Account");

        }

        [Authorize]
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> EditShelf(int id)
        {
            return Content("EditShelf Get");
        }

        [Authorize]
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> EditShelf(int id, ShelfDetailModel shelfModel)
        {
            return Content("EditShelf Post");
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteShelf(int id)
        {
            return Content("EditShelf Post");
        }

        [Authorize]
        [HttpGet("access-control/{id}")]
        public async Task<IActionResult> AccessControl(int id)
        {
            return Content("EditShelf Post");
        }

        [Authorize]
        [HttpPost("access-control/{id}")]
        public async Task<IActionResult> AccessControl(int id, ShelfDetailModel shelfModel)
        {
            return Content("EditShelf Post");
        }
    }
}
