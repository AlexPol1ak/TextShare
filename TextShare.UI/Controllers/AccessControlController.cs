﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.AccessRuleModels;
using TextShare.Domain.Utils;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Контроллер для управления правилами доступа.
    /// </summary>
    [Authorize]
    [Route("access-control")]
    public class AccessControlController : Controller
    {
        private readonly IShelfService _shelfService;
        private readonly IAccessRuleService _accessRuleService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly IFriendshipService _friendshipService;
        private readonly UserManager<User> _userManager;
        private readonly IAccessСontrolService _accessСontrolService;
        private readonly ITextFileService _textFileService;

        public AccessControlController(
            IShelfService shelfService,
            IAccessRuleService accessRuleService,
            IUserService userService,
            IGroupService groupService,
            IFriendshipService friendshipService,
            UserManager<User> userManager,
            IAccessСontrolService accessСontrolService,
            ITextFileService textFileService
            )
        {
            _shelfService = shelfService;
            _accessRuleService = accessRuleService;
            _userService = userService;
            _groupService = groupService;
            _friendshipService = friendshipService;
            _userManager = userManager;
            _accessСontrolService = accessСontrolService;
            _textFileService = textFileService;
        }

        [HttpGet("shelf-{shelfId}/edit")]
        public async Task<IActionResult> ShelfAccessRuleEdit(int shelfId)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;

            ResponseData<ShelfAccessRuleEditModel?> responseData = await CreateAccessRuleEditModel(
                currentUser.Id, shelfId
                );
            if(responseData.Success == false || responseData.Data == null )
            {
                HttpContext.Items["ErrorMessage"] = responseData.ErrorMessage;
                return BadRequest();
            }

            ShelfAccessRuleEditModel accessRuleEditModel = responseData.Data;
            return View(accessRuleEditModel);
        }

        

        [HttpPost("shelf-{shelfId}/edit")]
        public async Task<IActionResult> ShelfAccessRuleEdit(int shelfId,
            ShelfAccessRuleEditModel accessRuleEditModel)
        {

            User currentUser = (await _userManager.GetUserAsync(User))!;

            if (!ModelState.IsValid)
            {
                ResponseData<ShelfAccessRuleEditModel?> responseData = await CreateAccessRuleEditModel(
                    currentUser.Id, shelfId
                    );
                if (responseData.Success == false || responseData.Data == null)
                {
                    HttpContext.Items["ErrorMessage"] = responseData.ErrorMessage;
                    return BadRequest();
                }

                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    foreach (var error in value.Errors)
                    {
                        DebugHelper.ShowData($"Ошибка в {modelStateKey}: {error.ErrorMessage}");
                    }
                }

                ShelfAccessRuleEditModel accessRuleEditModelnew = responseData.Data;
                DebugHelper.ShowData(ModelState.IsValid);
                return View(accessRuleEditModelnew);
            }
            DebugHelper.ShowData(accessRuleEditModel.AvailableGroupIds.Count);

            Shelf? shelf = await _shelfService.GetShelfByIdAsync(shelfId,
                s => s.Creator,
                s => s.AccessRule, s => s.AccessRule.AvailableUsers, s => s.AccessRule.AvailableGroups
                );

            if (shelf == null)
            {
                HttpContext.Items["ErrorMessage"] = "Полка не найдена.";
                return NotFound();
            }         
        
            if (shelf.CreatorId != currentUser.Id)
            {
                HttpContext.Items["ErrorMessage"] = "У вас нет прав для управления доступом этой полки.";
                return NotFound();
            }
            AccessRule? accessRule = await _accessRuleService.GetAccessRuleByIdAsync(accessRuleEditModel.AccessRuleId);
            if (accessRule == null)
            {
                HttpContext.Items["ErrorMessage"] = "Настройки доступа не найдены.";
                return NotFound();
            }

            // Изменение "Доступно всем"
            accessRule.AvailableAll = accessRuleEditModel.AvailableAll;

            // Изменение "Доступно пользователям
            List<User> AvailableUsers = await _userService.FindUsersAsync(
                u => accessRuleEditModel.AvailableUserIds.Any(id => id == u.Id)
                );
            accessRule.AvailableUsers = AvailableUsers;

            // Изменение "Доступно группам"
            List<Group> AvailableGroups = await _groupService.FindGroupsAsync(
                g => accessRuleEditModel.AvailableGroupIds.Any(id => id == g.GroupId)
                );
            accessRule.AvailableGroups = AvailableGroups;

            await _accessRuleService.UpdateAccessRuleAsync(accessRule);
            await _accessRuleService.SaveAsync();

            DebugHelper.ShowData(accessRuleEditModel.ApplyToFiles);
            DebugHelper.ShowData(accessRuleEditModel.AvailableAll);

            if (accessRuleEditModel.ApplyToFiles)
            {
                List<TextFile> files = (await _textFileService.FindTextFilesAsync(
                    t => t.ShelfId == shelf.ShelfId,
                    t => t.AccessRule
                    )).ToList();

                foreach(TextFile file in files)
                {
                    int oldAccessRule = file.AccessRule.AccessRuleId;

                    AccessRule accessRuleShelfCopy = await _accessСontrolService.GetCopyAccessRule(accessRule);
                    accessRuleShelfCopy.TextFile = file;
                    accessRuleShelfCopy.TextFileId = file.TextFileId;
                    await _accessRuleService.CreateAccessRuleAsync(accessRuleShelfCopy);
                    await _accessRuleService.SaveAsync();

                    file.AccessRule = accessRuleShelfCopy; 
                    file.AccessRuleId = accessRuleShelfCopy.AccessRuleId;
                    await _textFileService.UpdateTextFileAsync(file);
                    await _accessRuleService.DeleteAccessRuleAsync(oldAccessRule);
                }
                await _textFileService.SaveAsync();
            }

            return RedirectToAction("DetailShelf","Shelves", new { id=shelf.ShelfId});
        }

        /// <summary>
        /// Создает модель для изменения правила доступа полки.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="shelfId"></param>
        /// <returns></returns>
        private async Task<ResponseData<ShelfAccessRuleEditModel?>> CreateAccessRuleEditModel(int userId, int shelfId)
        {
            ResponseData<ShelfAccessRuleEditModel?> responseData = new();

            Shelf? shelf = await _shelfService.GetShelfByIdAsync(shelfId,
                s => s.Creator,
                s => s.AccessRule, s => s.AccessRule.AvailableUsers, s => s.AccessRule.AvailableGroups
                );

            if (shelf == null)
            {
                responseData.Success = false;
                responseData.ErrorMessage = "Полка не найдена.";
                responseData.Data = null;
                return responseData;
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            if (shelf.CreatorId != currentUser.Id)
            {
                responseData.ErrorMessage = "У вас нет прав для управления доступом этой полки.";
                responseData.Data = null;
                responseData.Success = false;
                return responseData;
            }

            List<User> currentUserFriends = await _friendshipService.GetFriendsUser(currentUser.Id);
            List<Group> currentUserGroups = (await _groupService.GetUserCreatedGroupsAsync(currentUser.Id)).ToList();
            currentUserGroups.AddRange(await _groupService.GetUserMemberGroupsAsync(currentUser.Id));

            ShelfAccessRuleEditModel accessRuleEditModel = new();
            accessRuleEditModel.ShelfId = shelf.ShelfId;
            accessRuleEditModel.ShelfName = shelf.Name;
            accessRuleEditModel.AccessRuleId = shelf.AccessRuleId;
            accessRuleEditModel.AvailableAll = shelf.AccessRule.AvailableAll;
            accessRuleEditModel.AvailableUsers = shelf.AccessRule.AvailableUsers.ToList();
            accessRuleEditModel.AvailableGroups = shelf.AccessRule.AvailableGroups.ToList();
            accessRuleEditModel.CreatorUserFriends = currentUserFriends;
            accessRuleEditModel.CreatorUserGroups = currentUserGroups.ToList();

            responseData.Success = true;
            responseData.Data = accessRuleEditModel;
            return responseData;
        }
    }
}
