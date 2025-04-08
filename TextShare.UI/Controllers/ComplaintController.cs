using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Entities.Groups;
using Microsoft.AspNetCore.Mvc.Rendering;
using TextShare.Domain.Entities.Complaints;
using TextShare.Domain.Utils;
using Microsoft.CodeAnalysis.Text;

namespace TextShare.UI.Controllers
{
    [Authorize]
    public class ComplaintController : Controller
    {
        private readonly IShelfService _shelfService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly ITextFileService _textFileService;
        private readonly IAccessСontrolService _accessControlService;
        private readonly UserManager<User> _userManager;
        private readonly IComplaintService _complaintService;
        private readonly IComplaintReasonService _complaintReasonService;
        public ComplaintController(
            IShelfService shelfService,
            IUserService userService,
            IGroupService groupService,
            ITextFileService textFileService,
            IAccessСontrolService accessControlService,
            UserManager<User> userManager,
            IComplaintService complaintService,
            IComplaintReasonService complaintReasonService)
        {
            _shelfService = shelfService;
            _userService = userService;
            _groupService = groupService;
            _textFileService = textFileService;
            _accessControlService = accessControlService;
            _userManager = userManager;
            _complaintService = complaintService;
            _complaintReasonService = complaintReasonService;
        }

        [HttpGet("complaint/add/shelf-{shelfId:int}")]
        [HttpGet("complaint/add/group-{groupId:int}")]
        [HttpGet("complaint/add/file-{uniquename}")]
        public async Task<IActionResult> Add(int? shelfId, int? groupId, string? uniquename)
        {
            User currentUser = (await _userManager.GetUserAsync(User))!;

            string message = string.Empty;

            if (shelfId != null)
            {
                ResponseData<object> resp = await CanAddComplaintToShelfAsync(shelfId.Value, currentUser);
                if (resp.Success != true)
                {
                    HttpContext.Items["ErrorMessage"] = resp.ErrorMessage;
                    if (resp.Data is int code && code == 404) return NotFound();
                    return BadRequest();
                }

                if (resp.Data is Shelf shelf)
                {
                    string name = shelf.Name.Count() > 20 ? shelf.Name.Substring(0, 20) : shelf.Name;
                    message = $"Жалоба на полку\n{name}";
                }
            }
            else if (groupId != null)
            {
                ResponseData<object> resp = await CanAddComplaintToShelfAsync(groupId.Value, currentUser);
                if (resp.Success != true)
                {
                    HttpContext.Items["ErrorMessage"] = resp.ErrorMessage;
                    if (resp.Data is int code && code == 404) return NotFound();
                    return BadRequest();
                }

                if (resp.Data is Group group)
                {
                    string name = group.Name.Count() > 1 ? group.Name.Substring(0, 20) : group.Name;
                    message = $"Жалоба на полку\n{name}";
                }
            }
            else if (uniquename != null)
            {
                ResponseData<object> resp = await CanAddComplaintToFileAsync(uniquename, currentUser);
                if (resp.Success != true)
                {
                    HttpContext.Items["ErrorMessage"] = resp.ErrorMessage;
                    if (resp.Data is int code && code == 404) return NotFound();
                    return BadRequest();
                }

                if (resp.Data is TextFile file)
                {
                    string name = file.OriginalFileName.Count() > 1
                        ? file.OriginalFileName.Substring(0, 20) : file.OriginalFileName;
                    message = $"Жалоба на полку\n{name}";
                }
            }
            else
            {
                HttpContext.Items["ErrorMessage"] = "Неизвестная ошибка";
                return BadRequest();
            }

            if (string.IsNullOrEmpty(message))
            {
                HttpContext.Items["ErrorMessage"] = "Неизвестная ошибка";
                return BadRequest();
            }

            List<ComplaintReasons> complaintReasons = await _complaintReasonService.GetAllComplaintReasonsAsync();
            List<SelectListItem> SelectListItems = complaintReasons
                .Select(r => new SelectListItem
                {
                    Value = r.ComplaintReasonsId.ToString(),
                    Text = r.Name
                })
                .ToList();

            ViewBag.ComplaintReasons = complaintReasons;
            ViewBag.SelectListItem = SelectListItems;
            ViewBag.Message = message;

            return View();
        }

        [HttpPost("complaint/add/shelf-{shelfId:int}")]
        [HttpPost("complaint/add/group-{groupId:int}")]
        [HttpPost("complaint/add/file-{uniquename}")]
        public async Task<IActionResult> Add(int SelectedReasonId, int? shelfId, int? groupId, string? uniquename)
        {
            if (!ModelState.IsValid)
            {
                return await Add(shelfId, groupId, uniquename);
            }

            ComplaintReasons? reason = await _complaintReasonService.GetComplaintReasonByIdAsync(SelectedReasonId);
            if (reason == null)
            {
                HttpContext.Items["ErrorMessage"] = "Некорректная причина жалобы";
                return BadRequest();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            Complaint complaint = new();
            complaint.Author = currentUser;
            complaint.AuthorId = currentUser.Id;

            if (shelfId != null)
            {
                ResponseData<object> resp = await CanAddComplaintToShelfAsync(shelfId.Value, currentUser);
                if (resp.Success != true)
                {
                    HttpContext.Items["ErrorMessage"] = resp.ErrorMessage;
                    if (resp.Data is int code && code == 404) return NotFound();
                    return BadRequest();
                }

                if (resp.Data is Shelf shelf)
                {
                    complaint.Shelf = shelf;
                    complaint.ShelfId = shelf.ShelfId;
                }
            }
            else if (groupId != null)
            {
                ResponseData<object> resp = await CanAddComplaintToShelfAsync(groupId.Value, currentUser);
                if (resp.Success != true)
                {
                    HttpContext.Items["ErrorMessage"] = resp.ErrorMessage;
                    if (resp.Data is int code && code == 404) return NotFound();
                    return BadRequest();
                }

                if (resp.Data is Group group)
                {
                    complaint.Group = group;
                    complaint.GroupId = group.GroupId;
                }
            }
            else if (uniquename != null)
            {
                ResponseData<object> resp = await CanAddComplaintToFileAsync(uniquename, currentUser);
                if (resp.Success != true)
                {
                    HttpContext.Items["ErrorMessage"] = resp.ErrorMessage;
                    if (resp.Data is int code && code == 404) return NotFound();
                    return BadRequest();
                }

                if (resp.Data is TextFile file)
                {
                    complaint.TextFile = file;
                    complaint.TextFileId = file.TextFileId;
                }
            }
            else
            {
                HttpContext.Items["ErrorMessage"] = "Неизвестная ошибка";
                return BadRequest();

            }

            complaint.ComplaintReasons = reason;
            complaint.ComplaintReasonsId = reason.ComplaintReasonsId;
            await _complaintService.CreateComplaintAsync(complaint);
            await _complaintService.SaveAsync();

            if(shelfId != null)
                return RedirectToAction("DetailShelf", "Shelves", new { Id = shelfId });
            if(groupId != null)
                return RedirectToAction("DetailGroup", "Groups", new { groupId = groupId });
            if(uniquename != null)
                return RedirectToAction("DetailTextFile", "TextFile",
                   new { uniquename = uniquename });

            return RedirectToAction("Index", "Home");
        }

        #region Check access methods
         
        private async Task<ResponseData<object>> CanAddComplaintToShelfAsync(int shelfId, User currentUser)
        {
            var shelf = await _shelfService.GetShelfByIdAsync(shelfId,
                s => s.Complaints,
                s => s.Creator,
                s => s.AccessRule,
                s => s.AccessRule.AvailableGroups,
                s => s.AccessRule.AvailableUsers
            );

            if (shelf == null)
                return new ResponseData<object>(404, false, "Полка не найдена");

            if (shelf.CreatorId == currentUser.Id)
                return new ResponseData<object>(400, false, "Нельзя оставлять жалобу на свои полки.");

            if (await _accessControlService.CheckShelfAccess(currentUser, shelf) != true)
                return new ResponseData<object>(400, false, "У вас нет доступа к этой полке.");

            if (shelf.Complaints.Any(c => c.AuthorId == currentUser.Id))
                return new ResponseData<object>(406, false, "Вы уже оставляли жалобу");

            return new ResponseData<object>(shelf, true);
        }

        /// <summary>
        /// Проверяет, может ли текущий пользователь оставить жалобу на указанную группу.
        /// </summary>
        /// <param name="groupId">Идентификатор группы.</param>
        /// <param name="currentUser">Текущий пользователь.</param>
        /// <returns>
        /// Объект <see cref="ResponseData{Object}"/>, содержащий результат проверки.
        /// </returns>
        private async Task<ResponseData<object>> CanAddComplaintToGroupAsync(int groupId, User currentUser)
        {
            var group = await _groupService.GetGroupByIdAsync(groupId,
                g => g.Complaints,
                g => g.Creator,
                g => g.Members
            );

            if (group == null)
                return new ResponseData<object>(404, false, "Группа не найдена");

            if (group.CreatorId == currentUser.Id)
                return new ResponseData<object>(400, false, "Нельзя оставлять жалобу на свои группы.");

            var memberIds = group.Members
                .Where(m => m.IsConfirmed)
                .Select(m => m.UserId);

            if (!memberIds.Contains(currentUser.Id))
                return new ResponseData<object>(400, false, "У вас нет доступа к этой группе.");

            if (group.Complaints.Any(c => c.AuthorId == currentUser.Id))
                return new ResponseData<object>(406, false, "Вы уже оставляли жалобу на эту группу.");

            return new ResponseData<object>(group, true);
        }

        /// <summary>
        /// Проверяет, может ли текущий пользователь оставить жалобу на указанный файл.
        /// </summary>
        /// <param name="uniqueName">Уникальное имя файла без расширения.</param>
        /// <param name="currentUser">Текущий пользователь.</param>
        /// <returns>
        /// Объект <see cref="ResponseData{Object}"/>, содержащий результат проверки.
        /// </returns>
        private async Task<ResponseData<object>> CanAddComplaintToFileAsync(string uniqueName, User currentUser)
        {
            var file = (await _textFileService.FindTextFilesAsync(
                f => f.UniqueFileNameWithoutExtension == uniqueName,
                f => f.Complaints,
                f => f.AccessRule,
                f => f.AccessRule.AvailableGroups,
                f => f.AccessRule.AvailableUsers
            )).FirstOrDefault();

            if (file == null)
                return new ResponseData<object>(404, false, "Файл не найден");

            if (file.OwnerId == currentUser.Id)
                return new ResponseData<object>(400, false, "Нельзя оставлять жалобу на собственный файл.");

            if (await _accessControlService.CheckTextFileAccess(currentUser, file) != true)
                return new ResponseData<object>(400, false, "У вас нет доступа к этому файлу.");

            if (file.Complaints.Any(c => c.AuthorId == currentUser.Id))
                return new ResponseData<object>(406, false, "Вы уже оставляли жалобу на этот файл.");

            return new ResponseData<object>(file, true);
        }
        #endregion

    }
}
