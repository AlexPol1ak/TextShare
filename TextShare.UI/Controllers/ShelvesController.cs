using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels;

namespace TextShare.UI.Controllers
{
    [Route("shelves")]
    public class ShelvesController : Controller
    {
        private readonly IShelfService _shelfService;
        private readonly UserManager<User> _userManager;

        public ShelvesController(IShelfService shelfService, UserManager<User> userManager)
        {
            _shelfService = shelfService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> MyShelves()
        {
            ResponseData<ListModel<Shelf>> responseData = new();
            ListModel<Shelf> listModel = new ListModel<Shelf>();

            User user = (await _userManager.GetUserAsync(User))!;
            List<Shelf> userSheles = await _shelfService.GetAllUserShelvesAsync(user.Id);

            listModel.Items = userSheles;
            responseData.Data = listModel;
            
            return View(responseData);
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
        public async Task<IActionResult> CreateShelf(ShelfModel shelfModel)
        {
            return Content("Create Shelf Post");
        }


        [HttpGet("detail/{id}")]
        public async Task<IActionResult> DetailShelf(int id)
        {
            return Content("DetailShelf");
        }

        [Authorize]
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> EditShelf(int id)
        {
            return Content("EditShelf Get");
        }

        [Authorize]
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> EditShelf(int id, ShelfModel shelfModel)
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
        public async Task<IActionResult> AccessControl(int id, ShelfModel shelfModel)
        {
            return Content("EditShelf Post");
        }
    }
}
