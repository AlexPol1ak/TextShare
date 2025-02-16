using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Net;
using System.Runtime.CompilerServices;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.AccessRules;
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
        private readonly ShelvesSettings _shelvesSettings;
        private readonly UserManager<User> _userManager;


        public ShelvesController(IShelfService shelfService,
            UserManager<User> userManager,
            IAccessRuleService accessRuleService,
            IUserService userService,
            IOptions<ShelvesSettings> shelvesSettingsOptions,
            IFriendshipService friendshipService
            )
        {
            _shelfService = shelfService;
            _userManager = userManager;
            _accessRuleService = accessRuleService; 
            _userService = userService;
            _shelvesSettings = shelvesSettingsOptions.Value;
            _friendshipService = friendshipService;

            
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
            List<Shelf> userShelesAll = await _shelfService.GetAllUserShelvesAsync(user.Id);

            IEnumerable<Shelf> userAllShelvesE = userShelesAll.AsEnumerable();
            IPagedList<Shelf> shelvesPart = userAllShelvesE.ToPagedList(page, pageSize);
                  
            return View(shelvesPart);
        }

        /// <summary>
        /// Отображет страницу полок, к которым передставили  доступ друзья.
        /// </summary>
        /// <returns></returns>
        /// <remarks>shelves/friends-shared?page=1</remarks>
        [Authorize]
        [HttpGet("friends-shared")]
        public async Task<IActionResult> AvailableFromFriends(int page=1)
        {

            User user = (await _userManager.GetUserAsync(User))!;
            int pageSize = _shelvesSettings.MaxNumberShelvesInPage;

            // Получаем список друзей
            List<Friendship> userFriendships = await _friendshipService.GetFriendshipsByUserIdAsync(user.Id);

            if (userFriendships.Count == 0)
            {
                IPagedList<Shelf> responseEmptyShelves = new List<Shelf>().ToPagedList(page,pageSize);
                return View(responseEmptyShelves);
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

            return View(pagedShelves);
        }

        [Authorize]
        [HttpGet("shared-from-groups")]
        public async Task<IActionResult> AvailableFromGroups()
        {
            return Content("AvailableFromGroups");
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search()
        {
            return Content("Search");
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
