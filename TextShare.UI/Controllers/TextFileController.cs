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
using TextShare.Domain.Settings;
using TextShare.Domain.Utils;
using TextShare.UI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TextShare.UI.Controllers
{
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

        [Authorize]
        [HttpGet("upload/shelf-{shelfId}/upload")]
        public async Task<IActionResult> Upload(int shelfId)
        {
            Shelf? shelf = await _shelfService.GetShelfByIdAsync(shelfId,
                s=>s.TextFiles, s=>s.Creator
                );

            ResponseData<IActionResult> result = await canFileUpload(shelf);
            if (result.Success == false && result.Data != null)
            {
                HttpContext.Items["ErrorMessage"] = result.ErrorMessage;
                return result.Data;
            }

            List<Category> categories = await _categoryService.GetAllCategoriesAsync();

            var model = new FilesUploadModel
            {
                Categories = categories,
                ShelfId = shelf.ShelfId,
                ShelfName = shelf.Name,
                AllowedExtensionsStr = string.Join(',', _fileUploadSettings.AllowedExtensions)
            };

            return View(model);

        }

        [Authorize]
        [HttpPost("upload/shelf-{shelfId}/upload")]
        public async Task<IActionResult> Upload(int shelfId, FilesUploadModel filesUploadModel)
        {
            Shelf? shelf = await _shelfService.GetShelfByIdAsync(shelfId,
                s => s.TextFiles, s => s.Creator, s=>s.AccessRule,
                s=>s.AccessRule.AvailableUsers, s=>s.AccessRule.AvailableGroups
                );
            User currentUser = (await _userManager.GetUserAsync(User))!;

            ResponseData<IActionResult> result = await canFileUpload(shelf);
            if (result.Success == false && result.Data != null)
            {
                HttpContext.Items["ErrorMessage"] = result.ErrorMessage;
                return result.Data;
            }

            // Если в случае ошибок форма будет возращена клиенту.
            filesUploadModel.Categories = await _categoryService.GetAllCategoriesAsync();
            

            if (!ModelState.IsValid)
            {
                return View(filesUploadModel);
            }


            if (filesUploadModel.File == null || filesUploadModel.File.Length == 0)
            {
                ModelState.AddModelError("File", "Файл обязателен.");
                return View(filesUploadModel);
            }

            if (string.IsNullOrEmpty(filesUploadModel.Description))
            {
                ModelState.AddModelError("Description", "Описание обязательно.");
                return View(filesUploadModel);

            }

            if(filesUploadModel.SelectedCategoryIds ==null || !filesUploadModel.SelectedCategoryIds.Any())
            {
                ModelState.AddModelError("SelectedCategoryIds", "Выберите хотя бы одну категорию.");
                return View(filesUploadModel);
            }

            ResponseData<string> validateResponseData = await validateFile(filesUploadModel.File);
            if(!validateResponseData.Success && validateResponseData.Data != null)
            {
                //DebugHelper.ShowError(validateResponseData.ErrorMessage);
                ModelState.AddModelError("File", validateResponseData.ErrorMessage);
                return View(filesUploadModel);
            }

            ResponseData<Dictionary<string, string>> saveResponseData = new();
            saveResponseData = await saveFile(filesUploadModel.File);
            if(saveResponseData.Success == false || saveResponseData.Data == null)
            {
                //DebugHelper.ShowError(saveResponseData.ErrorMessage);
                ModelState.AddModelError("File", saveResponseData.ErrorMessage);
                return View(filesUploadModel);
            }

            Dictionary<string, string> fileData = saveResponseData.Data;

            TextFile textFile = new();
            textFile.OriginalFileName = fileData["originalFileName"];
            textFile.UniqueFileName = fileData["uniqueFileName"];
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

            AccessRule accessRule = await _accessСontrolService.GetCopyAccessRule(shelf.AccessRule);
            await _accessRuleService.CreateAccessRuleAsync(accessRule);
            await _accessRuleService.SaveAsync();
            textFile.AccessRule = accessRule;
            textFile.AccessRuleId = accessRule.AccessRuleId;

            await _textFileService.CreateTextFileAsync(textFile);
            await _textFileService.SaveAsync();


            return RedirectToAction("DetailTextFile", new { uniquename = textFile.UniqueFileName });
        }

        [HttpGet("{uniquename}")]
        public async Task<IActionResult> DetailTextFile(string uniquename)
        {
            return Content(uniquename);
        }

        public async Task<IActionResult> Download()
        {
            return View();
        }

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

        private async Task<bool> DeleteFileByUniqueName(string uniqueFileName)
        {
            return true;
        }

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
