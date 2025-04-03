using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using TextShare.Business.Interfaces;
using TextShare.Business.Services;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.ShelfModels;
using TextShare.Domain.Models.EntityModels.TextFileModels;
using TextShare.Domain.Settings;
using TextShare.Domain.Utils;
using TextShare.UI.Models;
using X.PagedList.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Контроллер для управления файлами
    /// </summary>
    [Route("file")]
    public class TextFileController : BaseController
    {
        private readonly FileUploadSettings _fileUploadSettings;
        private readonly UserManager<User> _userManager;
        private readonly ShelvesSettings _shelvesSettings;
        private readonly IUserService _userService;
        private readonly IShelfService _shelfService;
        private readonly ICategoryService _categoryService;
        private readonly IAccessСontrolService _accessСontrolService;
        private readonly IAccessRuleService _accessRuleService;
        private readonly ITextFileService _textFileService;
        public TextFileController(
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadSettingsOptions,
            IOptions<FileUploadSettings> fileUploadSettings,
            IOptions<ShelvesSettings> shelvesSettingsOptions,
            IShelfService shelfService,
            UserManager<User> userManager,
            IUserService userService,
            ICategoryService categoryService,
            IAccessСontrolService accessСontrolService,
            IAccessRuleService accessRuleService,
            ITextFileService textFileService
            ) 
            : base(physicalFile, imageUploadSettingsOptions)
        {
            _fileUploadSettings = fileUploadSettings.Value;
            _shelfService = shelfService;
            _userManager = userManager;
            _userService = userService;
            _shelvesSettings = shelvesSettingsOptions.Value;
            _categoryService = categoryService;
            _accessСontrolService = accessСontrolService;
            _accessRuleService = accessRuleService;
            _textFileService = textFileService;
               
        }

        /// <summary>
        /// Отображает страницу загрузки файла.
        /// </summary>
        /// <param name="shelfId">Id полки</param>
        /// <returns>Страницу загрузки файла.</returns>
        [Authorize]
        [HttpGet("upload/shelf-{shelfId}/upload")]
        public async Task<IActionResult> Upload(int shelfId)
        {
            // Найти полку
            Shelf? shelf = await _shelfService.GetShelfByIdAsync(shelfId,
                s=>s.TextFiles, s=>s.Creator
                );

            //Проверяет доступна ли загрузка файла на эту полку
            ResponseData<IActionResult> result = await canFileUpload(shelf);
            if (result.Success == false && result.Data != null)
            {
                HttpContext.Items["ErrorMessage"] = result.ErrorMessage;
                return result.Data;
            }

            List<Category> categories = await _categoryService.GetAllCategoriesAsync();

            // Заполнение модели загрузки.
            var model = new FilesUploadModel
            {
                Categories = categories,
                ShelfId = shelf.ShelfId,
                ShelfName = shelf.Name,
                AllowedExtensionsStr = string.Join(',', _fileUploadSettings.AllowedExtensions)
            };

            return View(model);
        }

        /// <summary>
        /// Обрабатывает POST запрос загрузки файла на полку
        /// </summary>
        /// <param name="shelfId">Id полки</param>
        /// <param name="filesUploadModel">Модель загрузки файла</param>
        /// <returns>Сохраняет файл и перенаправляет на страницу файла.</returns>
        [Authorize]
        [HttpPost("upload/shelf-{shelfId}/upload")]
        public async Task<IActionResult> Upload(int shelfId, FilesUploadModel filesUploadModel)
        {
            // Получить полку
            Shelf? shelf = await _shelfService.GetShelfByIdAsync(shelfId,
                s => s.TextFiles, s => s.Creator, s=>s.AccessRule,
                s=>s.AccessRule.AvailableUsers, s=>s.AccessRule.AvailableGroups
                );
            User currentUser = (await _userManager.GetUserAsync(User))!;

            // Проверить разрешена ли загрузка на полку
            ResponseData<IActionResult> result = await canFileUpload(shelf);
            if (result.Success == false && result.Data != null)
            {
                HttpContext.Items["ErrorMessage"] = result.ErrorMessage;
                return result.Data;
            }

            // Если в случае ошибок форма будет возращена клиенту -заполнить категории заново
            filesUploadModel.Categories = await _categoryService.GetAllCategoriesAsync();
            
            if (!ModelState.IsValid)
            {
                return View(filesUploadModel);
            }

            // Проверка наличия файла
            if (filesUploadModel.File == null || filesUploadModel.File.Length == 0)
            {
                ModelState.AddModelError("File", "Файл обязателен.");
                return View(filesUploadModel);
            }

            // Проверка описания файла
            if (string.IsNullOrEmpty(filesUploadModel.Description))
            {
                ModelState.AddModelError("Description", "Описание обязательно.");
                return View(filesUploadModel);

            }

            // Проверка выбора категории
            if(filesUploadModel.SelectedCategoryIds ==null || !filesUploadModel.SelectedCategoryIds.Any())
            {
                ModelState.AddModelError("SelectedCategoryIds", "Выберите хотя бы одну категорию.");
                return View(filesUploadModel);
            }

            // Проверка допустимости файла
            ResponseData<string> validateResponseData = await validateFile(filesUploadModel.File);
            if(!validateResponseData.Success && validateResponseData.Data != null)
            {
                ModelState.AddModelError("File", validateResponseData.ErrorMessage);
                return View(filesUploadModel);
            }

            // Сохранение файла на сервере.
            ResponseData<Dictionary<string, string>> saveResponseData = new();
            saveResponseData = await saveFile(filesUploadModel.File);
            if(saveResponseData.Success == false || saveResponseData.Data == null)
            {
                ModelState.AddModelError("File", saveResponseData.ErrorMessage);
                return View(filesUploadModel);
            }

            Dictionary<string, string> fileData = saveResponseData.Data;

            // Сохранение данных в базу
            TextFile textFile = new();
            textFile.OriginalFileName = fileData["originalFileName"];
            textFile.UniqueFileName = fileData["uniqueFileName"];
            textFile.UniqueFileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileData["uniqueFileName"]);
            textFile.Extention = fileData["type"];
            textFile.Description = filesUploadModel.Description;     
            textFile.ContentType = filesUploadModel.File.ContentType;
            textFile.Size = filesUploadModel.File.Length;
            textFile.Uri = fileData["uri"];
            textFile.Owner = currentUser;
            textFile.OwnerId = currentUser.Id;
            textFile.Shelf = shelf;
            textFile.ShelfId = shelf.ShelfId;

            List<Category> categories =await _categoryService.FindCategoriesAsync(
                f => filesUploadModel.SelectedCategoryIds.Any(id => id == f.CategoryId)
                );
            textFile.TextFileCategories = categories.Select(
                c=> new TextFileCategory() { Category = c, TextFile = textFile }
                ).ToList();

            // Правило доступа для файла копируется от правила доступа для полки.
            AccessRule accessRule = await _accessСontrolService.GetCopyAccessRule(shelf.AccessRule);
            await _accessRuleService.CreateAccessRuleAsync(accessRule);
            await _accessRuleService.SaveAsync();
            textFile.AccessRule = accessRule;
            textFile.AccessRuleId = accessRule.AccessRuleId;

            await _textFileService.CreateTextFileAsync(textFile);
            await _textFileService.SaveAsync();

            return RedirectToAction("DetailTextFile", new { uniquename = textFile.UniqueFileNameWithoutExtension });
        }

        /// <summary>
        /// Отображает страницу с детальной информацией о файле
        /// </summary>
        /// <param name="uniquename"></param>
        /// <returns></returns>

        [HttpGet("{uniquename}")]
        public async Task<IActionResult> DetailTextFile(string uniquename)
        {
            TextFile? textFile = (await _textFileService.FindTextFilesAsync(
                t => t.UniqueFileNameWithoutExtension == uniquename,
                t=>t.Owner, t=>t.TextFileCategories, t=>t.Shelf
                )).FirstOrDefault();          

            if(textFile == null)
            {
                HttpContext.Items["ErrorMessage"] = "Файл не найден";
                return BadRequest();
            }

            List<int> fileCategoriesIds = textFile.TextFileCategories.Select(c => c.CategoryId).ToList();
            List<Category> fileCategories = await _categoryService.FindCategoriesAsync(
                cat => fileCategoriesIds.Any(id => id == cat.CategoryId)
                );

            User? currentUser  = await _userManager.GetUserAsync(User);
            TextFileDetailModel model = TextFileDetailModel.FromTextFile(textFile, currentUser?.Id ?? null );
            model.Categories = fileCategories;


            return View(model);
        }

        /// <summary>
        /// Скачивание файла
        /// </summary>
        /// <param name="uniquename"></param>
        /// <returns></returns>
        [HttpGet("download/{uniquename}")]
        public async Task<IActionResult> Download(string uniquename)
        {
            List<TextFile> textFiles = await _textFileService.FindTextFilesAsync(
                t => t.UniqueFileNameWithoutExtension == uniquename,
                t => t.AccessRule, t => t.AccessRule.AvailableGroups, t => t.AccessRule.AvailableUsers,
                t => t.Owner
                );
            if(textFiles.Count < 0)
            {
                HttpContext.Items["ErrorMessage"] = "Файл не найден";
                return BadRequest();

            }
            TextFile textFile = textFiles[0];
            User? currentUser = await _userManager.GetUserAsync(User);
            var result = await _accessСontrolService.CheckTextFileAccess(currentUser, textFile);
            if(result != true)
            {
                HttpContext.Items["ErrorMessage"] = "У вас нет доступа к этому файлу";
                return BadRequest();
            }

            Dictionary<string, string?> filePaths = await _physicalFile.GetFile(
                textFile.UniqueFileName, "TextFiles"
                );
            string? relativePath = filePaths.GetValueOrDefault("relativePath");

            if(relativePath == null)
            {
                HttpContext.Items["ErrorMessage"] = "Файл не найден!";
                return BadRequest();
            }
            return new VirtualFileResult(relativePath, textFile.ContentType) 
            { FileDownloadName = textFile.OriginalFileName };
        }

        /// <summary>
        /// Удаление файла.
        /// </summary>
        /// <param name="uniquename"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("delete/{uniquename}")]
        public async Task<IActionResult> Delete(string uniquename)
        {
            var files = await _textFileService.FindTextFilesAsync(
                t=>t.UniqueFileNameWithoutExtension == uniquename,
                t=>t.Shelf);
            if(files.Count() < 1)
            {
                HttpContext.Items["ErrorMessage"] = "Файл не найден";
                return BadRequest();
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            TextFile textFile = files[0];
            int shelfId = textFile.ShelfId;
            
            if(textFile.OwnerId !=  currentUser.Id)
            {
                HttpContext.Items["ErrorMessage"] = "Нет разрешения на действие в этим файлом!";
                return BadRequest();
            }

            var res = await _physicalFile.Delete(textFile.UniqueFileName, "TextFiles");
            await _textFileService.DeleteTextFileAsync(textFile.TextFileId);
            await _textFileService.SaveAsync();

            return RedirectToAction("DetailShelf", "Shelves", new {id=shelfId});
        }


        /// <summary>
        /// Отображает страницу с файлами доступными всем.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{username}/available-all")]
        public async Task<IActionResult> UserFilesAvvAll(string username, int page = 1)
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

            List<TextFile> files = await _textFileService.FindTextFilesAsync(
                t => t.OwnerId == viewedUser.Id && t.AccessRule.AvailableAll == true
                );
            List<TextFileDetailShortModel> filesModel = await  TextFileDetailShortModel.FromTextFiles(files);
            ViewData["viewedUsername"] = viewedUser.UserName;

            return View(filesModel.ToPagedList(page, 5));
        }

        /// <summary>
        /// Проверяет загружаемый файл в систему
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<ResponseData<string>> validateFile(IFormFile file)
        {
            await Task.CompletedTask;
            ResponseData<string> responseData = new();
            responseData.Success = true;

            string mimeTypes = file.ContentType;
            string ext = Path.GetExtension(file.FileName);
            string fileName = file.FileName;
            long size = file.Length;

            if (!_fileUploadSettings.AllowedMimeTypes.Contains(mimeTypes))
            {
                responseData.Success = false;
                responseData.ErrorMessage = "Неверный тип файла";
                return responseData;
            }
            if (!_fileUploadSettings.AllowedExtensions.Contains(ext))
            {
                responseData.Success = false;
                responseData.ErrorMessage = $"Неверное расширение.";              
                return responseData;
            }
            if (size > _fileUploadSettings.MaxFileSize)
            {
                int megabytes = (int)(_imageUploadSettings.MaxFileSize / (1024 * 1024));

                responseData.Success = false;
                responseData.ErrorMessage = $"Слишком  большой размер файла." +
                    $" Файл должен быть до {megabytes} Mb.";
                return responseData;
            }

            return responseData;
        }

        /// <summary>
        /// Сохраняет загруженный файл в файловой системе и возвращает информацию о сохранении.
        /// </summary>
        /// <param name="file">Файл, загруженный пользователем.</param>
        /// <returns>
        /// Объект <see cref="ResponseData{Dictionary{string, string}}"/> с данными о сохранении файла:<br/>
        /// - "relativePath": относительный путь к файлу <br/>
        /// - "uri": полный URI для доступа к файлу <br/>
        /// - "originalFileName": оригинальное имя файла с расширением <br/>
        /// - "uniqueFileName": уникальное имя файла с расширением <br/>
        /// - "size": размер в байтах <br/>
        /// - "type": расширение <br/>
        /// </returns>
        private async Task<ResponseData<Dictionary<string, string>>> saveFile(IFormFile file)
        {
            ResponseData<Dictionary<string, string>> responseData = new();           

            Dictionary<string, string> dataDict = new();
            try
            {
                using (Stream fileStreame = file.OpenReadStream())
                {
                    dataDict = await _physicalFile.Save(fileStreame, file.FileName, "TextFiles");
                }
            }
            catch
            {
                responseData.Data = null;
                responseData.Success = false;
                responseData.ErrorMessage = "Не удалось сохранить изображение.";
                return responseData;
            }

            string uri = await GetFileUri(dataDict["relativePath"]);
            dataDict.Add("uri", uri);
            responseData.Data = dataDict;
            return responseData;
        }

        /// <summary>
        /// Проверяет возможность загрузки файла на полку.
        /// </summary>
        /// <param name="shelf">Полка</param>
        /// <returns></returns>
        private async Task<ResponseData<IActionResult>> canFileUpload(Shelf? shelf)
        {
            ResponseData<IActionResult> responseData = new() { Success = true };

            if (shelf == null)
            {
                responseData.ErrorMessage = "Полка не найдена";
                responseData.Success = false;
                responseData.Data = BadRequest();
                return responseData;
            }

            User currentUser = (await _userManager.GetUserAsync(User))!;
            if (shelf.CreatorId != currentUser.Id)
            {
                responseData.ErrorMessage = "Нельзя загружать файлы на чужие полки";
                responseData.Success = false;
                responseData.Data = BadRequest();
                return responseData;
            }

            if (shelf.TextFiles.Count() >= _shelvesSettings.MaxFilesInShelf)
            {
                responseData.ErrorMessage = $"Полка '{shelf.Name}' переполнена";
                responseData.Success = false;
                responseData.Data = BadRequest();
                return responseData;
            }

            return responseData;

        }
    }
}
