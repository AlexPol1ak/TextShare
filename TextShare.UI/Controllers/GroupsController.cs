using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.GroupModels;
using TextShare.Domain.Models.EntityModels.ShelfModels;
using TextShare.Domain.Settings;
using X.PagedList.Extensions;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Контроллер для управления группами.
    /// </summary>
    [Authorize]
    [Route("groups")]
    public class GroupsController : BaseController
    {
        private readonly GroupsSettings _groupsSettings;

        private readonly UserManager<User> _userManager;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        public GroupsController(
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadSettingsOptions,
            UserManager<User> userManager,
            IGroupService groupService,
            IUserService userService,
            IOptions<GroupsSettings> groupsSettings
            ) 
            : base(physicalFile, imageUploadSettingsOptions)
        {
            _userManager = userManager;
            _groupService = groupService;
            _userService = userService;
            _groupsSettings = groupsSettings.Value;
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyGroups(int page = 1)
        {

            User currentUser = (await _userManager.GetUserAsync(User))!;
            List<Group> groups = new();
            groups.AddRange(
                (await _groupService.GetUserCreatedGroupsAsync(currentUser.Id, g=>g.Creator))
                .OrderBy(g=>g.CreatedAt)
                );
            groups.AddRange(
                (await _groupService.GetUserMemberGroupsAsync(currentUser.Id, g => g.Creator, g => g.Members))
                .OrderBy(g=>g.Members.Select(m=>m.JoinedAt))
                );

            List<GroupDetailModel> groupDetailsList = (await GroupDetailModel.FromGroup(groups))
                .Select(
                model =>
                {
                    if (model.Creator.Id == currentUser.Id) model.UserRelationStatus = UserRelationStatus.Creator;
                    else model.UserRelationStatus = UserRelationStatus.Member;
                    return model;
                }         
                ).ToList();


            return View(groupDetailsList.ToPagedList(page, _groupsSettings.MaxGroupInPage));
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> UserGroups(string username,int page = 1)
        {
            return Content("");
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchGroups(int page = 1)
        {
            return Content("");
        }

        /// <summary>
        /// Страница создания новой группы
        /// </summary>
        /// <returns>Страница создания группы.</returns>
        ///<remarks>POST groups/create-group</remarks>
        [HttpGet("create")]
        public async Task<IActionResult> CreateGroup()
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;

            int countUserGroups = (await _groupService.GetUserCreatedGroupsAsync(currentUser.Id)).Count;
            if (countUserGroups >= _groupsSettings.MaxGroupsCreate)
            {
                HttpContext.Items["ErrorMessage"] = "Вы организовали максимально допустимое количество групп";
                return BadRequest();
            }

            GroupCreateModel groupCreateModel = new();

            return View(groupCreateModel);
        }

        /// <summary>
        /// Обрабатывает POST запрос создания новой группы
        /// </summary>
        /// <returns>Перенаправляет на страницу созданной группы</returns>
        ///<remarks>POST groups/create-group</remarks>
        [HttpPost("create-group")]   
        public async Task<IActionResult> CreateGroup(GroupCreateModel groupCreateModel, IFormFile? AvatarFile)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;

            int countUserGroups = (await _groupService.GetUserCreatedGroupsAsync(currentUser.Id)).Count;
            if(countUserGroups >= _groupsSettings.MaxGroupsCreate)
            {
                HttpContext.Items["ErrorMessage"] = "Вы организовали максимально допустимое количество групп";
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(groupCreateModel);
            }

            Group newGroup = groupCreateModel.ToGroup();
            newGroup.Creator = currentUser;
            newGroup.CreatorId = currentUser.Id;
            
            if(AvatarFile != null)
            {
                ResponseData<Dictionary<string, string>> data = new();
                data = await SaveImage(AvatarFile);

                if (data.Success == false && data.ErrorMessage != null)
                {
                    ModelState.AddModelError("AvataError", data.ErrorMessage);
                    return View(groupCreateModel);
                }

                newGroup.ImageUri = data.Data.GetValueOrDefault("uri", null);
                newGroup.MimeType = AvatarFile.ContentType;
            }

            newGroup = await  _groupService.CreateGroupAsync(newGroup);
            await _groupService.SaveAsync();

            return RedirectToAction("DetailGroup", new { groupId = newGroup.GroupId });

        }

        [HttpPost("update")]
        [HttpGet("update")]
        public async Task<IActionResult> UpdateGroup()
        {
            return Content("");
        }

        [HttpGet("delete")]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteGroup()
        {
            return Content("");
        }

        [HttpGet("group-{groupId}")]
        public async Task<IActionResult> DetailGroup(int groupId)
        {
            return Content($"{groupId}");
        }

        public async Task<IActionResult> AvailableShelvesGroup()
        {
            return Content("");
        }

        public async Task<IActionResult> AvailableFilesGroup()
        {
            return Content("");
        }

        public async Task<IActionResult> GroupMembers()
        {
            return Content("");
        }

        public async Task<IActionResult> RequestsUsersGroup()
        {
            return Content("");
        }

        public async Task<IActionResult> OutRequests()
        {
            return Content("");
        }

        public async Task<IActionResult> AcceptRequest()
        {
            return Content("");
        }

        public async Task<IActionResult> DeleteRequestGroup()
        {
            return Content("");
        }


    }
}
