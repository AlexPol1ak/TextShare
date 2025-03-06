using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TextShare.Business.Interfaces;
using TextShare.Business.Services;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.GroupModels;
using TextShare.Domain.Models.EntityModels.ShelfModels;
using TextShare.Domain.Settings;
using TextShare.Domain.Utils;
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
        private readonly IFriendshipService _friendshipService;
        public GroupsController(
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadSettingsOptions,
            UserManager<User> userManager,
            IGroupService groupService,
            IUserService userService,
            IOptions<GroupsSettings> groupsSettings,
            IFriendshipService friendshipService
            ) 
            : base(physicalFile, imageUploadSettingsOptions)
        {
            _userManager = userManager;
            _groupService = groupService;
            _userService = userService;
            _groupsSettings = groupsSettings.Value;
            _friendshipService = friendshipService;
        }

        [HttpGet()]
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
                    if (model.Creator.Id == currentUser.Id) model.UserGroupRelationStatus = UserGroupRelationStatus.Creator;
                    else model.UserGroupRelationStatus = UserGroupRelationStatus.Member;
                    return model;
                }         
                ).ToList();


            return View(groupDetailsList.ToPagedList(page, _groupsSettings.MaxGroupInPage));
        }

        /// <summary>
        /// Возвращает страницу с списком групп определенного пользователя.
        /// </summary>
        /// <param name="username">Username просматриваемого пользователя</param>
        /// <param name="page"></param>
        /// <returns>Страницу с списком групп пользователя.</returns>
        /// <remarks> GET groups/{username}?page=1</remarks>
        [HttpGet("{username}")]
        public async Task<IActionResult> UserGroups(string username,int page = 1)
        {
            // Если пустой username
            if (string.IsNullOrEmpty(username))
            {
                HttpContext.Items["ErrorMessage"] = "Не корректная ссылка.";
                return BadRequest();
            }

            // Текущий пользователь ( который получает страницу)
            User currentUser = (await _userManager.GetUserAsync(User))!;

            // Просматриваемый пользователь 
            User? viewedUser = await _userService.GetUserByUsernameAsync(username);           
            // Если просматриваемый пользователь не найден.
            if (viewedUser ==null)
            {
                HttpContext.Items["ErrorMessage"] = "Не корректная ссылка.";
                return BadRequest();
            }

            // Список друзей просматриваемого пользователя.
            List<User> viewedUserFriends = await _friendshipService.GetFriendsUser(viewedUser.Id);
            // Если  текущий пол-ль не дружит с просматр. поль-лем - перенаправить на страницу
            if((viewedUserFriends.Any(u=>u.Id == currentUser.Id)))
            {
                return RedirectToAction("UserDetail", "User", new {username = viewedUser.UserName});
            }

            // Список созданных групп просмтр. пользователем.
            List<Group> viewedUserGroups = await _groupService.GetUserCreatedGroupsAsync(viewedUser.Id, g=>g.Members);
            // Список групп, в которых просмотр. поль-ль состоит.
            viewedUserGroups.AddRange(await _groupService.GetUserMemberGroupsAsync(viewedUser.Id, g => g.Members));

            // Модель групп для View();
            List<GroupDetailModel> groupDetailsList = (await GroupDetailModel.FromGroup(viewedUserGroups))
                .Select(
                model =>
                {
                    // Установка отношений в модели текущего пользователя к группам в списке
                    Group? group = viewedUserGroups.Find(g => g.GroupId == model.GroupId);
                    if (group == null) return model;

                    if (group.CreatorId == currentUser.Id)
                        model.UserGroupRelationStatus = UserGroupRelationStatus.Creator;
                    else if (group.Members.Any(m => m.UserId == currentUser.Id && m.IsConfirmed == true))
                        model.UserGroupRelationStatus = UserGroupRelationStatus.Member;
                    else if (group.Members.Any(m => m.UserId == currentUser.Id && m.IsConfirmed == false))
                        model.UserGroupRelationStatus = UserGroupRelationStatus.Requsted;
                    else
                        model.UserGroupRelationStatus = UserGroupRelationStatus.NotMember;
                        return model;
                }
                ).ToList();

            return View(groupDetailsList.ToPagedList(page, _groupsSettings.MaxGroupInPage));
        }       

        [HttpGet("search")]
        public async Task<IActionResult> SearchGroups([FromQuery] string? groupName = null, int page = 1)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;
            User userDb = (await _userService.GetUserByIdAsync(currentUser.Id, u => u.GroupMemberships))!;
            List<int> userGroupRequest = userDb.GroupMemberships.Where(m => m.IsConfirmed == false).Select(m => m.GroupId).ToList();
            List<int> userGroupsMember = userDb.GroupMemberships.Where(m => m.IsConfirmed == true).Select(m => m.GroupId).ToList();

            if (string.IsNullOrEmpty(groupName) || string.IsNullOrWhiteSpace(groupName))
            {
                return View("SearchGroupsInput");
            }

            List<Group> resultSearsh = new();
            resultSearsh = await _groupService.FindGroupsAsync(
                g => g.Name.Contains(groupName),
                g=>g.Creator,
                g=>g.Members
                );
            
            

            List<GroupDetailModel> groupDetailsList = (await GroupDetailModel.FromGroup(resultSearsh))
                .Select(
                model =>
                {
                    if (model.Creator.Id == currentUser.Id) model.UserGroupRelationStatus = UserGroupRelationStatus.Creator;
                    else if (userGroupRequest.Any(
                        id => id == model.GroupId)) model.UserGroupRelationStatus = UserGroupRelationStatus.Requsted;
                    else if (userGroupsMember.Any(
                        id => id == model.GroupId)) model.UserGroupRelationStatus = UserGroupRelationStatus.Member;
                    else
                        model.UserGroupRelationStatus = UserGroupRelationStatus.NotMember;
                    return model;
                }              
                ).ToList();

            return View("SearchGroupsOutput", groupDetailsList.ToPagedList(page, _groupsSettings.MaxGroupInPage));
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
        [HttpPost("create")]   
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

        [HttpGet("out-requests")]
        public async Task<IActionResult> OutRequests(int page=1)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;
            List<Group> outRequestGroups = await _groupService.GetUserOutRequestsGroups(currentUser.Id);

            List<GroupDetailModel> groupDetailsList = (await GroupDetailModel.FromGroup(outRequestGroups))
                .Select(
                model => { model.UserGroupRelationStatus = UserGroupRelationStatus.Requsted; return model; }
                ).ToList();

            return View(groupDetailsList.ToPagedList(page,_groupsSettings.MaxGroupInPage));
        }

        [HttpGet("delete")]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g=>g.Creator);
            if(group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            if(Request.Method == HttpMethods.Get)
            {
                if(group.CreatorId != currentUser.Id)
                {
                    HttpContext.Items["ErrorMessage"] = "У вас нет прав для управления  этой группой";
                    return BadRequest();
                }
                GroupDeleteModel groupDeleteModel = GroupDeleteModel.FromGroup(group);
                return View(groupDeleteModel);                               
            }

            if(Request.Method == HttpMethods.Post)
            {
                if (group.CreatorId != currentUser.Id)
                {
                    HttpContext.Items["ErrorMessage"] = "У вас нет прав для управления  этой группой";
                    return BadRequest();
                }

                if(group.ImageUri != null)
                {
                    await this.DeleteImageByUri(group.ImageUri);
                }
                await _groupService.DeleteGroupAsync(group.GroupId);
            }

            return RedirectToAction("MyGroups");
        }

        [HttpGet("group-{groupId}")]
        public async Task<IActionResult> DetailGroup(int groupId)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Members, g=>g.Creator);
            if(group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            GroupDetailModel groupDetailModel = await GroupDetailModel.FromGroup(group);

            if (group.CreatorId == currentUser.Id)
                groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.Creator;
            else if (group.Members.Any(m => m.UserId == currentUser.Id && m.IsConfirmed == true))
                groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.Member;
            else if (group.Members.Any(m => m.UserId == currentUser.Id && m.IsConfirmed == false))
                groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.Requsted;
            else
                groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.NotMember;

            return View(groupDetailModel);
        }

        [HttpGet("group-{groupId}/shelves")]
        public async Task<IActionResult> AvailableShelves(int groupId, int page = 1)
        {
            return Content("");
        }

        [HttpGet("group-{groupId}/files")]
        public async Task<IActionResult> AvailableFiles(int groupId, int page = 1)
        {
            return Content("");
        }

        [HttpGet("group-{groupId}/members")]
        public async Task<IActionResult> GroupMembers(int groupId, int page=1)
        {
            return Content("");
        }

        [HttpGet("group-{groupId}/in-requests")]
        public async Task<IActionResult> RequestsUsers(int groupId, int page = 1)
        {
            return Content("");
        }

        [HttpPost("group-{groupId}/update")]
        [HttpGet("group-{groupId}/update")]
        public async Task<IActionResult> UpdateGroup(int groupId)
        {
            return Content("");
        }


        public async Task<IActionResult> AcceptRequest()
        {
            return Content("");
        }

        public async Task<IActionResult> DeleteRequest()
        {
            return Content("");
        }


    }
}
