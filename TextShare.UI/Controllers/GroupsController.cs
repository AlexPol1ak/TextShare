using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.GroupModels;
using TextShare.Domain.Models.EntityModels.ShelfModels;
using TextShare.Domain.Models.EntityModels.TextFileModels;
using TextShare.Domain.Models.EntityModels.UserModels;
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
        private readonly ITextFileService _textFileService;
        private readonly IShelfService _shelfService;
        private readonly IAccessСontrolService _accessControlService;
        public GroupsController(
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadSettingsOptions,
            UserManager<User> userManager,
            IGroupService groupService,
            IUserService userService,
            IOptions<GroupsSettings> groupsSettings,
            IFriendshipService friendshipService,
            ITextFileService textFileService,
            IShelfService shelfService,
            IAccessСontrolService accessСontrolService
            )
            : base(physicalFile, imageUploadSettingsOptions)
        {
            _userManager = userManager;
            _groupService = groupService;
            _userService = userService;
            _groupsSettings = groupsSettings.Value;
            _friendshipService = friendshipService;
            _textFileService = textFileService;
            _shelfService = shelfService;
            _accessControlService = accessСontrolService;
        }

        /// <summary>
        /// Отображает страницу с списком групп пользователя.
        /// </summary>
        /// <param name="page"></param>
        /// <returns>Страница с списком групп пользователя</returns>
        [HttpGet()]
        public async Task<IActionResult> MyGroups(int page = 1)
        {

            User currentUser = (await _userManager.GetUserAsync(User))!;
            List<Group> groups = new();
            groups.AddRange(
                (await _groupService.GetUserCreatedGroupsAsync(currentUser.Id, g => g.Creator))
                .OrderBy(g => g.CreatedAt)
                );
            groups.AddRange(
                (await _groupService.GetUserMemberGroupsAsync(currentUser.Id, g => g.Creator, g => g.Members))
                .OrderBy(g => g.Members.Min(m => m.JoinedAt))
            );

            List<GroupDetailModel> groupDetailsList = (await GroupDetailModel.FromGroup(groups))
                .Select(
                model =>
                {
                    if (model.Creator.Id == currentUser.Id) model.UserGroupRelationStatus = UserGroupRelationStatus.Creator;
                    else model.UserGroupRelationStatus = UserGroupRelationStatus.Member;
                    return model;
                }
                ).DistinctBy(g => g.GroupId).ToList();


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
        public async Task<IActionResult> UserGroups(string username, int page = 1)
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
            if (viewedUser == null)
            {
                HttpContext.Items["ErrorMessage"] = "Не корректная ссылка.";
                return BadRequest();
            }

            // Список друзей просматриваемого пользователя.
            List<User> viewedUserFriends = await _friendshipService.GetFriendsUser(viewedUser.Id);
            // Если  текущий пол-ль не дружит с просматр. поль-лем - перенаправить на страницу
            if ((viewedUserFriends.Any(u => u.Id == currentUser.Id)))
            {
                return RedirectToAction("UserDetail", "User", new { username = viewedUser.UserName });
            }

            // Список созданных групп просмтр. пользователем.
            List<Group> viewedUserGroups = await _groupService.GetUserCreatedGroupsAsync(viewedUser.Id, g => g.Members);
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

        /// <summary>
        /// Отображает страницу поиска
        /// </summary>
        /// <param name="groupName">Название группы</param>
        /// <param name="page">Номер страницы поиска</param>
        /// <returns>Страница поиска</returns>
        /// <remarks>GET search?groupName=null?page=1</remarks>
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
                g => g.Creator,
                g => g.Members
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
            if (countUserGroups >= _groupsSettings.MaxGroupsCreate)
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

            if (AvatarFile != null)
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

            GroupMember member = new();
            member.Group = newGroup;
            member.User = currentUser;
            member.IsConfirmed = true;

            newGroup.Members.Add(member);

            newGroup = await _groupService.CreateGroupAsync(newGroup);
            await _groupService.SaveAsync();

            return RedirectToAction("DetailGroup", new { groupId = newGroup.GroupId });

        }

        /// <summary>
        /// Отображает страницу с исходящими заявками пользователя.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("out-requests")]
        public async Task<IActionResult> OutRequests(int page = 1)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;
            List<Group> outRequestGroups = await _groupService.GetUserOutRequestsGroups(currentUser.Id);

            List<GroupDetailModel> groupDetailsList = (await GroupDetailModel.FromGroup(outRequestGroups))
                .Select(
                model => { model.UserGroupRelationStatus = UserGroupRelationStatus.Requsted; return model; }
                ).ToList();

            return View(groupDetailsList.ToPagedList(page, _groupsSettings.MaxGroupInPage));
        }

        /// <summary>
        /// Обрабатывает Post запрос присоединения к новой группе
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>Перенаправляет на группу</returns>
        [HttpPost("join-group")]
        public async Task<IActionResult> JoinGroup(int groupId, string? returnUrl = null)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Members, g => g.Creator);
            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;

            if (group.Members == null)
            {
                group.Members = new List<GroupMember>();
            }

            if (!group.Members.Any(m => m.UserId == currentUser.Id))
            {
                GroupMember groupMember = new()
                {
                    User = currentUser,
                    Group = group
                };

                group.Members.Add(groupMember);
                await _groupService.UpdateGroupAsync(group);
                await _groupService.SaveAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("DetailGroup", "Groups", new { groupId });
        }


        /// <summary>
        /// Обрабатывает POST и GET запросы удаления группы.
        /// Администратор может удалить любую группу без ограничений,
        /// обычный пользователь — только свою.
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns>Результат удаления и перенаправление</returns>
        [Authorize]
        [HttpGet("delete")]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Creator);
            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }

            bool isAdmin = User.IsInRole("Admin");
            User currentUser = (await _userManager.GetUserAsync(User))!;

            if (Request.Method == HttpMethods.Get)
            {
                if (!isAdmin && group.CreatorId != currentUser.Id)
                {
                    HttpContext.Items["ErrorMessage"] = "У вас нет прав для управления этой группой";
                    return BadRequest();
                }

                GroupDeleteModel groupDeleteModel = GroupDeleteModel.FromGroup(group);
                return View(groupDeleteModel);
            }

            if (Request.Method == HttpMethods.Post)
            {
                if (!isAdmin && group.CreatorId != currentUser.Id)
                {
                    HttpContext.Items["ErrorMessage"] = "У вас нет прав для управления этой группой";
                    return BadRequest();
                }

                if (group.ImageUri != null)
                {
                    await this.DeleteImageByUri(group.ImageUri);
                }

                await _groupService.DeleteGroupAsync(group.GroupId);
                await _groupService.SaveAsync();
            }

            // Перенаправление в зависимости от роли
            if (isAdmin)
                return RedirectToAction("ViewComplaints", "Complaint", new { type = "groups" });
            else
                return RedirectToAction("MyGroups");
        }

        /// <summary>
        /// Отображает страницу с детальной информацией о группе.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>Страница с информацией о группе.</returns>
        /// <remarks>GET groups/group-{groupId}</remarks>
        [HttpGet("group-{groupId}")]
        public async Task<IActionResult> DetailGroup(int groupId)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Members, g => g.Creator);
            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            bool isAdmin = User.IsInRole("Admin");
            GroupDetailModel groupDetailModel = await GroupDetailModel.FromGroup(group);

            if (group.CreatorId == currentUser.Id)
                groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.Creator;
            else if ((group.Members.Any(m => m.UserId == currentUser.Id && m.IsConfirmed == true)) || isAdmin == true)
                groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.Member;
            else if (group.Members.Any(m => m.UserId == currentUser.Id && m.IsConfirmed == false))
                groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.Requsted;
            else
                groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.NotMember;

            return View(groupDetailModel);
        }

        /// <summary>
        /// Отображает страницу с участниками группы.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("group-{groupId}/members")]
        public async Task<IActionResult> GroupMembers(int groupId, int page = 1)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Creator, g => g.Members);
            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }
            User currentUser = (await _userManager.GetUserAsync(User))!;
            List<int> membersIds = group.Members.Where(m => m.IsConfirmed == true).Select(m => m.UserId).ToList();

            if (!membersIds.Any(id => currentUser.Id == id))
            {
                HttpContext.Items["ErrorMessage"] = "Данную страницу могут просматривать только участники группы.";
                return BadRequest();

            }
            List<User> members = await _userService.FindUsersAsync(
                u => membersIds.Any(id => id == u.Id)
                );

            GroupMembersModel groupMembersModel = new();
            groupMembersModel.CurrentUser = UserModel.FromUser(currentUser);
            groupMembersModel.Group = await GroupDetailModel.FromGroup(group);
            if (group.CreatorId == currentUser.Id)
                groupMembersModel.Group.UserGroupRelationStatus = UserGroupRelationStatus.Creator;
            else
                groupMembersModel.Group.UserGroupRelationStatus = UserGroupRelationStatus.Member;

            groupMembersModel.Members = (await UserModel.FromUsers(members)).ToPagedList(page, 10);


            return View(groupMembersModel);
        }

        /// <summary>
        /// Отображает страницу с входящими заявками на встпление у группу.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("group-{groupId}/in-requests")]
        public async Task<IActionResult> RequestsUsers(int groupId, int page = 1)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Creator, g => g.Members);
            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }
            User currentUser = (await _userManager.GetUserAsync(User))!;
            List<int> membersIds = group.Members.Where(m => m.IsConfirmed == false).Select(m => m.UserId).ToList();
            List<User> members = await _userService.FindUsersAsync(
                u => membersIds.Any(id => id == u.Id)
                );

            GroupMembersModel groupMembersModel = new();
            groupMembersModel.CurrentUser = UserModel.FromUser(currentUser);
            groupMembersModel.Group = await GroupDetailModel.FromGroup(group);
            groupMembersModel.Members = (await UserModel.FromUsers(members)).ToPagedList(page, 10);


            return View(groupMembersModel);
        }

        /// <summary>
        /// Обрабатывает POST запрос одобрения заявки участия в группе.
        /// </summary>
        /// <param name="groupId">Id группы</param>
        /// <param name="username">Username пользователя</param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost("group-{groupId}/accept-request")]
        public async Task<IActionResult> AcceptRequest(int groupId, string username, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(username))
            {
                HttpContext.Items["ErrorMessage"] = $"Некорректный username";
                return BadRequest();
            }

            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Creator, g => g.Members);
            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            if (group.CreatorId != currentUser.Id)
            {
                HttpContext.Items["ErrorMessage"] = "Недостаточно полномочий";
                return BadRequest();
            }

            User? acceptUser = await _userService.GetUserByUsernameAsync(username);
            if (acceptUser == null)
            {
                HttpContext.Items["ErrorMessage"] = $"Пользователь {username} не найден";
                return BadRequest();
            }

            GroupMember? groupMember = group.Members.Where(
                g => g.UserId == acceptUser.Id && g.IsConfirmed == false
                ).FirstOrDefault();

            if (groupMember != null)
            {
                groupMember.IsConfirmed = true;
                await _groupService.UpdateGroupAsync(group);
                await _groupService.SaveAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("RequestsUsers", new { groupId });
        }

        /// <summary>
        /// Обрабатывает POST запрос на удаление пользователя из группы.
        /// </summary>
        /// <param name="groupId">Id группы</param>
        /// <param name="username"> username пользователя</param>
        /// <param name="returnUrl">Ссылка перенаправления</param>
        /// <returns></returns>
        [HttpPost("group-{groupId}/delete-member")]
        public async Task<IActionResult> DeleteMember(int groupId, string username, string? returnUrl = null)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Creator, g => g.Members);
            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }

            User? deleteUser = await _userService.GetUserByUsernameAsync(username);
            if (deleteUser == null)
            {
                HttpContext.Items["ErrorMessage"] = $"Пользователь {username} не найден";
                return BadRequest();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;

            if (group.CreatorId == currentUser.Id)
            {
                GroupMember? groupMember = group.Members.Where(m => m.UserId == deleteUser.Id).FirstOrDefault();
                if (groupMember != null)
                {
                    group.Members.Remove(groupMember);
                    await _groupService.UpdateGroupAsync(group);
                    await _groupService.SaveAsync();
                }
            }
            else
            {
                HttpContext.Items["ErrorMessage"] = "Вы не можете удалить этого пользователя из группы";
                return BadRequest();
            }


            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("GroupMembers", new { groupId });
        }

        /// <summary>
        /// Обрабатывает POST запрос выхода пользователя из группы или отмены заявки на вступление
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost("group -{groupId}/leave-group")]
        public async Task<IActionResult> LeaveGroup(int groupId, string? returnUrl = null)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Creator, g => g.Members);
            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;

            if (group.Members != null)
            {
                GroupMember? groupMember = group.Members.Where(m => m.UserId == currentUser.Id).FirstOrDefault();
                if (groupMember != null)
                {
                    group.Members.Remove(groupMember);
                    await _groupService.UpdateGroupAsync(group);
                    await _groupService.SaveAsync();
                }

            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("GroupMembers", new { groupId });
        }

        /// <summary>
        /// Отображает страницу обновления группы.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("group-{groupId}/update")]
        public async Task<IActionResult> UpdateGroup(int groupId)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Creator);
            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            if (group.CreatorId != currentUser.Id)
            {
                HttpContext.Items["ErrorMessage"] = "Нет права управления этой группок.";
                return BadRequest();
            }

            GroupUpdateModel groupUpdateModel = new();
            groupUpdateModel.GroupId = group.GroupId;
            groupUpdateModel.Name = group.Name;
            groupUpdateModel.Description = group.Description;
            groupUpdateModel.ImageUri = group.ImageUri;

            return View(groupUpdateModel);
        }

        /// <summary>
        /// Обрабатывает POST запрос обновления группы.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="model"></param>
        /// <param name="ImageFile"></param>
        /// <returns></returns>
        [HttpPost("group-{groupId}/update")]
        public async Task<IActionResult> UpdateGroup(int groupId, GroupUpdateModel model, IFormFile? ImageFile)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId, g => g.Creator);
            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return BadRequest();
            }

            // Проверяем, является ли пользователь создателем группы
            User currentUser = (await _userManager.GetUserAsync(User))!;
            if (group.CreatorId != currentUser.Id)
            {
                HttpContext.Items["ErrorMessage"] = "Нет права управления этой группой.";
                return BadRequest();
            }

            // Проверяем корректность модели
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Обновляем название и описание
            group.Name = model.Name;
            group.Description = model.Description;

            // Обрабатываем загрузку изображения (если загружен новый файл)
            if (ImageFile != null)
            {
                var result = await validateImage(ImageFile);
                if (!result.Success)
                {
                    ModelState.AddModelError("ImageFile", result.ErrorMessage);
                    return View(model);
                }

                ResponseData<Dictionary<string, string>> data = await SaveImage(ImageFile);
                if (data.Success && data.Data != null)
                {
                    // Удаляем старое изображение, если оно было
                    if (!string.IsNullOrEmpty(group.ImageUri))
                    {
                        await DeleteImageByUri(group.ImageUri);
                        group.ImageUri = null;
                        DebugHelper.ShowData(group.ImageUri);
                    }

                    group.ImageUri = data.Data["uri"];
                }
            }

            await _groupService.UpdateGroupAsync(group);
            await _groupService.SaveAsync();

            return RedirectToAction("DetailGroup", new { groupId = group.GroupId });
        }

        /// <summary>
        /// Возвращает страницу с полками доступными группе.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("group-{groupId}/shelves")]
        public async Task<IActionResult> AvailableShelves(int groupId, int page = 1)
        {

            Group? group = await _groupService.GetGroupByIdAsync(groupId,
                g => g.Members, g => g.Creator
                );

            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return NotFound();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            List<int> groupMemberIds = group.Members.
                Where(g => g.IsConfirmed == true)
                .Select(gm => gm.UserId)
                .ToList();
            if (!groupMemberIds.Any(id => id == currentUser.Id))
            {
                HttpContext.Items["ErrorMessage"] = "Недостаточно прав просматривать эту страницу.";
                return BadRequest();
            }

            List<Shelf> shelves = await _accessControlService.AvailableShelvesForGroup(
                group.GroupId, g => g.Creator
                );

            List<ShelfDetailShort> shelvesModels = await ShelfDetailShort.FromShelves(shelves);

            // Модель для боковых ссылок
            GroupDetailModel groupDetailModel = await GroupDetailModel.FromGroup(group);
            if (group.CreatorId == currentUser.Id) groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.Creator;
            else groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.Member;
            ViewBag.GroupDetailModel = groupDetailModel;

            return View(shelvesModels.ToPagedList(page, 5));
        }

        /// <summary>
        ///  Возвращает страницу с файлами доступными группе
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("group-{groupId}/files")]
        public async Task<IActionResult> AvailableFiles(int groupId, int page = 1)
        {
            Group? group = await _groupService.GetGroupByIdAsync(groupId,
               g => g.Members, g => g.Creator
               );

            if (group == null)
            {
                HttpContext.Items["ErrorMessage"] = "Группа не найдена";
                return NotFound();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            List<int> groupMemberIds = group.Members.
                Where(g => g.IsConfirmed == true)
                .Select(gm => gm.UserId)
                .ToList();
            if (!groupMemberIds.Any(id => id == currentUser.Id))
            {
                HttpContext.Items["ErrorMessage"] = "Недостаточно прав просматривать эту страницу.";
                return BadRequest();
            }

            // Модель для боковых ссылок
            GroupDetailModel groupDetailModel = await GroupDetailModel.FromGroup(group);
            if (group.CreatorId == currentUser.Id) groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.Creator;
            else groupDetailModel.UserGroupRelationStatus = UserGroupRelationStatus.Member;
            ViewBag.GroupDetailModel = groupDetailModel;

            List<TextFile> files = await _accessControlService.AvailableFilesForGroup(groupId, f => f.Owner);
            List<TextFileDetailShortModel> textFilesModels = await TextFileDetailShortModel.FromTextFiles(files);

            return View(textFilesModels.ToPagedList(page, 5));
        }



    }
}
