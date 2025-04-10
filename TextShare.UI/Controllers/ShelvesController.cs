using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.IO;
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
using TextShare.Domain.Models.EntityModels.TextFileModels;
using TextShare.Domain.Settings;
using TextShare.Domain.Utils;
using TextShare.UI.Models;
using X.PagedList;
using X.PagedList.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Контроллер для управления полками.
    /// </summary>
    [Route("shelves")]
    public class ShelvesController : BaseController
    {
        private readonly IShelfService _shelfService;
        private readonly IAccessRuleService _accessRuleService;       
        private readonly IUserService _userService;
        private readonly IFriendshipService _friendshipService;
        private readonly IGroupService _groupService;
        private readonly ShelvesSettings _shelvesSettings;
        private readonly UserManager<User> _userManager;
        private readonly IAccessСontrolService _accessСontrolService;

        public ShelvesController(IShelfService shelfService,
            UserManager<User> userManager,
            IAccessRuleService accessRuleService,
            IUserService userService,
            IOptions<ShelvesSettings> shelvesSettingsOptions,
            IFriendshipService friendshipService,
            IGroupService groupService,
            IAccessСontrolService accessСontrolService,
            // В базовый контроллер
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadSettingsOptions
            ) :base(physicalFile, imageUploadSettingsOptions)
        {
            _shelfService = shelfService;
            _userManager = userManager;
            _accessRuleService = accessRuleService; 
            _userService = userService;
            _shelvesSettings = shelvesSettingsOptions.Value;
            _friendshipService = friendshipService;
            _groupService = groupService;
            _accessСontrolService = accessСontrolService;
                     
        }

        #region Actions
        /// <summary>
        /// Отображает страницу с полками пользователя.
        /// </summary>
        /// <param name="page">Страница полок</param>
        /// <returns></returns>
        /// <remarks>GET shelves/my?page=1</remarks>
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
        /// Получает список всех полок пользователя, доступных всем, с учётом дружбы между пользователями.
        /// </summary>
        /// <param name="username">Имя пользователя, чьи полки будут отображаться.</param>
        /// <param name="page">Номер страницы для пагинации (по умолчанию 1).</param>
        /// <returns>Представление с моделью полок, доступных всем пользователям.</returns>
        [Authorize]
        [HttpGet("{username}/available-all")]
        public async Task<IActionResult> UserShelvesAvAll(string username, int page = 1)
        {
            User? viewedUser = await _userService.GetUserByUsernameAsync(username, u => u.Friendships);
            if (viewedUser == null)
            {
                HttpContext.Items["ErrorMessage"] = $"Пользователь \"{username}\" не найден";
                return NotFound();
            }
            User currentUser = (await _userManager.GetUserAsync(User))!;

            Friendship? frViewedUser = viewedUser.Friendships.Where(
                f => (f.UserId == currentUser.Id || f.FriendId == currentUser.Id) && f.IsConfirmed == true
                ).FirstOrDefault();
            if (frViewedUser == null)
            {
                HttpContext.Items["ErrorMessage"] = $"Вы не можете просматривать эту страницу";
                return NotFound();
            }

            // Все полки созданные пользователем и доступные всем.
            List<Shelf> shelvesViewedUser = (await _shelfService.GetAllUserShelvesAsync(viewedUser.Id, s=>s.AccessRule))
                .Where(s=>s.AccessRule.AvailableAll == true).ToList();

            ViewData["viewedUsername"] = viewedUser.UserName;
            return View(shelvesViewedUser.ToPagedList(page, 5));

        }

        /// <summary>
        /// Отображает страницу полок, к которым предоставили  доступ друзья.
        /// </summary>
        /// <returns></returns>
        /// <remarks>GET shelves/friends-shared?page=1</remarks>
        [Authorize]
        [HttpGet("friends-shared")]
        public async Task<IActionResult> AvailableFromFriends(int page=1)
        {

            User user = (await _userManager.GetUserAsync(User))!;
            int pageSize = _shelvesSettings.MaxNumberShelvesInPage;
            AvailableShelvesModel<User, IPagedList<Shelf>> responseModel = new();
            responseModel.User = user;

            // Получаем список друзей
            List <Friendship> userFriendships = await _friendshipService.GetAllUserAcceptedFriendshipAsync(user.Id);

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
        /// <remarks>GET shelves/shared-from-groups?page=1</remarks>
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
            List<Shelf> availableShelves = await _accessСontrolService.AvailableShelvesFromGroups(userDb.Id);

            AvailableShelvesModel<User, IPagedList<Shelf>> responseModel = new();
            responseModel.User = userDb;
            responseModel.AvailableShelves = availableShelves.ToPagedList(page, pageSize);
          
            return View(responseModel);
        }

        /// <summary>
        /// Отображает страницу поиска полок
        /// </summary>
        /// <param name="shelvesSearchModel">Модель запроса поиска полок</param>
        /// <param name="page">страница резульатата</param>
        /// <returns>Результат поиска</returns>
        /// <remarks>GET shelves/search?page=1</remarks>
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
            string? name = shelvesSearchModel.Name;
            string? description = shelvesSearchModel.Description;
            bool onlyAvailableMe = shelvesSearchModel.OnlyAvailableMe;
            List<Shelf> shelvesResult = new(); // Результат всего поиска
            List<Shelf> shelvesResponse = new(); // Результат фильтрации и ответ пользователю
            

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

        /// <summary>
        /// Отображает страницу создания новой полки.
        /// </summary>
        /// <returns>Страница создания полки</returns>
        /// <remarks>GET shelves/create-shelf</remarks>
        [Authorize]
        [HttpGet("create-shelf")]
        public async Task<IActionResult> CreateShelf()
        {
            // Текущий пользователь
            User user = (await _userManager.GetUserAsync(User))!;
            // Проверка достижения лимита количества  полок пользователя.
            var shelf = await _shelfService.GetAllUserShelvesAsync(user.Id);
            if (shelf.Count >= _shelvesSettings.MaxNumberUserShelves)
            {
                HttpContext.Items["ErrorMessage"] = 
                    $"Вы достигли максимального количества полок.\nМаксимум {_shelvesSettings.MaxNumberUserShelves}.";
                return BadRequest();
            }
               
            return View();
        }

        /// <summary>
        /// Обрабатывает POST запрос создания новой полки.
        /// </summary>
        /// <param name="shelfCreateModel">Модель создания полки</param>
        /// <param name="avatarFile">Аватар полки</param>
        /// <returns>Перенаправляет на страницу с полками</returns>
        /// <remarks>POST shelves/create-shelf</remarks>
        [Authorize]
        [HttpPost("create-shelf")]
        public async Task<IActionResult> CreateShelf(ShelfCreateModel shelfCreateModel, IFormFile? avatarFile)
        {
            if (!ModelState.IsValid)
            {
                return View(shelfCreateModel);
            }
            User user = (await _userManager.GetUserAsync(User))!;
            AccessRule shelfAccessRule = new();
            var taskCreateAccessRule = _accessRuleService.CreateAccessRuleAsync(shelfAccessRule);

            Shelf shelf = shelfCreateModel.ToShelf();        
            shelf.Creator = user;
            shelf.CreatorId = user.Id;
            if(avatarFile != null)
            {
                ResponseData<Dictionary<string, string>> data = new();
                data = await SaveImage(avatarFile);
                if(data.Success == false && data.ErrorMessage != null)
                {
                    ModelState.AddModelError("AvatarFile", data.ErrorMessage);
                    return View(shelfCreateModel);
                }
                string? avatarUri = data.Data.GetValueOrDefault("uri", null);
                shelf.ImageUri = avatarUri;
                shelf.MimeType = avatarFile.ContentType;                                          
            }
            shelfAccessRule = await taskCreateAccessRule;
            await _accessRuleService.SaveAsync();

            shelf.AccessRule = shelfAccessRule;
            shelf.AccessRuleId = shelfAccessRule.AccessRuleId;

            Shelf createdShelf = await _shelfService.CreateShelfAsync(shelf);
            await _shelfService.SaveAsync();

            return RedirectToAction("MyShelves");
        }

        /// <summary>
        /// Отображает детальную информацию о полке
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            User? currentUser = await _userManager.GetUserAsync(User);
            bool isAdmin = User.IsInRole("Admin");

            var result = await _accessСontrolService.CheckShelfAccess(currentUser, shelf);
            if (result == true || isAdmin ==true)
            {
                ShelfDetailModel shelfDetailModel = ShelfDetailModel.FromShelf(shelf);
                shelfDetailModel.CurrentUserId = currentUser?.Id ?? null;
                return View(shelfDetailModel);

            }else if(currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            HttpContext.Items["ErrorMessage"] = "У вас нет доступа к этой полке";
            return BadRequest(); ;
        }

        /// <summary>
        /// Отображает страницу редактирования полки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ///<remarks>GET shelves/edit/id</remarks>
        [Authorize]
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> EditShelf(int id)
        {

            Shelf? updateShelf = await _shelfService.GetShelfByIdAsync(id);
            if(updateShelf == null)
            {
                HttpContext.Items["ErrorMessage"] = "Такая полка не найдена";
                return NotFound();
            }

            User user = (await _userManager.GetUserAsync(User))!;
            if(updateShelf.CreatorId != user.Id)
            {
                HttpContext.Items["ErrorMessage"] = "Недостаточно прав";
                return NotFound();
            }

            ShelfCreateModel updateShelfModel = ShelfCreateModel.FromShelf(updateShelf);

            return View(updateShelfModel);
        }

        /// <summary>
        /// Обрабатывает POST запрос на редактирование полки
        /// </summary>
        /// <param name="id">Id полки</param>
        /// <param name="shelfCreateModel">Модель создания полки</param>
        /// <param name="AvatarFile">Изображение</param>
        /// <returns></returns>
        /// <remarks> POST shelves/edit/id</remarks>
        [Authorize]
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> EditShelf(int id, ShelfCreateModel shelfCreateModel, IFormFile? AvatarFile)
        {
            if (!ModelState.IsValid)
            {
                return View(shelfCreateModel);
            }
            Shelf? updateShelf = await _shelfService.GetShelfByIdAsync(id);
            if (updateShelf == null)
            {
                HttpContext.Items["ErrorMessage"] = "Такая полка не найдена";
                return NotFound();
            }

            User user = (await _userManager.GetUserAsync(User))!;
            if (updateShelf.CreatorId != user.Id)
            {
                HttpContext.Items["ErrorMessage"] = "Недостаточно прав";
                return Forbid();
            }

            updateShelf.Name = shelfCreateModel.Name;
            updateShelf.Description = shelfCreateModel.Description;

            if(AvatarFile != null)
            {
                var result = await validateImage(AvatarFile);
                if (!result.Success)
                {
                    ModelState.AddModelError("AvatarFile", result.ErrorMessage);
                    return View(shelfCreateModel);
                }
              
                ResponseData<Dictionary<string, string>> data = await SaveImage(AvatarFile);
                if (data.Success && data.Data != null)
                {
                    if (updateShelf.ImageUri != null)
                    {
                        await DeleteImageByUri(updateShelf.ImageUri);
                        updateShelf.ImageUri = null;
                    }
                    updateShelf.MimeType = AvatarFile.ContentType;
                    updateShelf.ImageUri = data.Data["uri"];
                }                           
            }
            await _shelfService.UpdateShelfAsync(updateShelf);
            await _shelfService.SaveAsync();

            return RedirectToAction("DetailShelf", new { id });
        }

        /// <summary>
        /// Отображает страницу удаления полки или удаляет полку
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]     
        [HttpGet("delete/{id}")]
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteShelf(int id)
        {
            // Получаем полку и файлы
            Shelf? shelf = await _shelfService.GetShelfByIdAsync(id, s => s.TextFiles);

            if (shelf == null)
            {
                HttpContext.Items["ErrorMessage"] = "Полка не найдена";
                return NotFound();
            }

            bool isAdmin = User.IsInRole("Admin");

            // Получаем текущего пользователя
            User user = (await _userManager.GetUserAsync(User))!;

            // Если не админ, проверяем, может ли пользователь удалить полку
            if (!isAdmin)
            {
                if (!shelf.CanDeleted)
                {
                    HttpContext.Items["ErrorMessage"] = "Эта полка не может быть удалена";
                    DebugHelper.ShowData(shelf);
                    return BadRequest();
                }

                if (shelf.CreatorId != user.Id)
                {
                    HttpContext.Items["ErrorMessage"] = "Вы не можете удалить чужую полку.";
                    return BadRequest();
                }
            }

            // Если метод GET — показать подтверждение удаления
            if (HttpContext.Request.Method == HttpMethods.Get)
            {
                ShelfDeleteModel shelfDelete = ShelfDeleteModel.FromShelf(shelf);
                return View(shelfDelete);
            }

            // Метод POST — удаление
            if (!string.IsNullOrEmpty(shelf.ImageUri))
            {
                await DeleteImageByUri(shelf.ImageUri);
            }

            List<string> filesUniqueFileNames = shelf.TextFiles.Select(t => t.UniqueFileName).ToList();
            foreach (var fileName in filesUniqueFileNames)
            {
                _physicalFile.Delete(fileName, "TextFiles");
            }

            await _shelfService.DeleteShelfAsync(shelf.ShelfId);
            await _shelfService.SaveAsync();

            // Перенаправление в зависимости от роли
            if (isAdmin)
                return RedirectToAction("ViewComplaints", "Complaint", new { type = "shelves" });
            else
                return RedirectToAction("MyShelves");
        }

        /// <summary>
        /// Отображает страницу с списком файлов, размещённых на полке, с учётом доступа пользователя.
        /// </summary>
        /// <param name="shelfId">Идентификатор полки.</param>
        /// <param name="page">Номер страницы для пагинации (по умолчанию 1).</param>
        /// <returns>Представление с моделью файлов, доступных пользователю, на указанной полке.</returns>
        [HttpGet("/shelf-{shelfId}/files")]
        public async Task<IActionResult> FilesInShelf(int shelfId, int page = 1)
        {
            Shelf? shelf = await _shelfService.GetShelfByIdAsync(shelfId,
                s=>s.TextFiles, s=>s.Creator,
                s=>s.AccessRule, s=>s.AccessRule.AvailableGroups, s=>s.AccessRule.AvailableUsers);

            if(shelf == null)
            {
                HttpContext.Items["ErrorMessage"] = "Полка не найдена";
                return BadRequest();
            }

            User? user = await _userManager.GetUserAsync(User);

            var result = await _accessСontrolService.CheckShelfAccess(user, shelf);
            if(result != true)
            {
                if(user == null)
                {
                    return Challenge();
                }
                HttpContext.Items["ErrorMessage"] = "У вас нет доступа к файлам на этой полке";
                return BadRequest();
            }

            List<TextFile> files = new();

            foreach (var t in shelf.TextFiles)
            {
                if (await _accessСontrolService.CheckTextFileAccess(user, t) == true)
                {
                    files.Add(t);
                }
            }
            List<TextFileDetailShortModel> models = await TextFileDetailShortModel.FromTextFiles(files);
            ViewBag.ShelfName = shelf.Name;


            return View(models.ToPagedList(page, 5));
        }
     
        #endregion       
    }
}
