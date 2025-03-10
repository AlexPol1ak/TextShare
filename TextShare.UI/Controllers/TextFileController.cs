﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using TextShare.Business.Interfaces;
using TextShare.Business.Services;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Settings;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TextShare.UI.Controllers
{
    [Route("files")]
    public class TextFileController : BaseController
    {
        private readonly FileUploadSettings _fileUploadSettings;
        private readonly UserManager<User> _userManager;
        private readonly ShelvesSettings _shelvesSettings;
        private readonly IUserService _userService;
        private readonly IShelfService _shelfService;
        private readonly ICategoryService _categoryService;
        public TextFileController(
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadSettingsOptions,
            IOptions<FileUploadSettings> fileUploadSettings,
            IOptions<ShelvesSettings> shelvesSettingsOptions,
            IShelfService shelfService,
            UserManager<User> userManager,
            IUserService userService,
            ICategoryService categoryService
            ) 
            : base(physicalFile, imageUploadSettingsOptions)
        {
            _fileUploadSettings = fileUploadSettings.Value;
            _shelfService = shelfService;
            _userManager = userManager;
            _userService = userService;
            _shelvesSettings = shelvesSettingsOptions.Value;
            _categoryService = categoryService;
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

            var model = new FileUploadModel
            {
                Categories = categories
            };

            ViewBag.Shelf = shelf;
            ViewBag.AllowedExtensionsStr = string.Join(',', _fileUploadSettings.AllowedExtensions);

            return View(model);

        }

        [Authorize]
        [HttpPost("upload/shelf-{shelfId}/upload")]
        public async Task<IActionResult> Upload(int shelfId, FileUploadModel fileUploadModel, IFormFile? file)
        {
            Shelf? shelf = await _shelfService.GetShelfByIdAsync(shelfId,
                s => s.TextFiles, s => s.Creator, s=>s.AccessRule,
                s=>s.AccessRule.AvailableUsers, s=>s.AccessRule.AvailableGroups
                );

            ResponseData<IActionResult> result = await canFileUpload(shelf);
            if (result.Success == false && result.Data != null)
            {
                HttpContext.Items["ErrorMessage"] = result.ErrorMessage;
                return result.Data;
            }

            if(file == null)
            {
                return View(fileUploadModel);
            }


            return Redirect("");
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
        private async Task<ResponseData<Dictionary<string, string>>> SaveFile(IFormFile file)
        {
            ResponseData<Dictionary<string, string>> responseData = new();

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
