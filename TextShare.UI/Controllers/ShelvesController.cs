using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;
using System.Runtime.CompilerServices;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.ShelfModels;
using TextShare.Domain.Utils;

namespace TextShare.UI.Controllers
{
    [Route("shelves")]
    public class ShelvesController : Controller
    {
        private readonly IShelfService _shelfService;
        private readonly IAccessRuleService _accessRuleService;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public ShelvesController(IShelfService shelfService, 
            UserManager<User> userManager,
            IAccessRuleService accessRuleService,
            IUserService userService
            )
        {
            _shelfService = shelfService;
            _userManager = userManager;
            _accessRuleService = accessRuleService; 
            _userService = userService;
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> MyShelves()
        {
            ListModel<Shelf> listModel = new ListModel<Shelf>();

            User user = (await _userManager.GetUserAsync(User))!;
            List<Shelf> userSheles = await _shelfService.GetAllUserShelvesAsync(user.Id);

            listModel.Items = userSheles;
            
            return View(listModel);
        }

        [Authorize]
        [HttpGet("friends-shared")]
        public async Task<IActionResult> AvailableFromFriends()
        {
            return Content("AvailableFromFriends");
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
