using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TextShare.Business.Interfaces;
using TextShare.Domain.Models;
using TextShare.Domain.Settings;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Базовый контроллер
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly ImageUploadSettings _imageUploadSettings;
        protected IPhysicalFile _physicalFile;

        public BaseController(
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadSettingsOptions
            ) : base()
        {
            _physicalFile = physicalFile;
            _imageUploadSettings = imageUploadSettingsOptions.Value;
        }

        #region Save Images
        protected async Task<string> GetFileUri(string relativePath)
        {
            await Task.CompletedTask;
            string normalizedPath = relativePath.Replace('\\', '/').TrimStart('/');
            string fullUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{normalizedPath}";
            return fullUrl;
        }

        protected async Task<ResponseData<string>> validateImage(IFormFile imageFormFile)
        {
            await Task.CompletedTask;

            ResponseData<string> data = new();
            data.Success = true;

            string mimeTypes = imageFormFile.ContentType;
            string ext = Path.GetExtension(imageFormFile.FileName);
            string fileName = imageFormFile.FileName;
            long size = imageFormFile.Length;

            if (!_imageUploadSettings.AllowedMimeTypes.Contains(mimeTypes))
            {
                data.Success = false;
                data.ErrorMessage = "Неверный тип изображения";
                return data;
            }
            if (!_imageUploadSettings.AllowedExtensions.Contains(ext))
            {
                data.Success = false;
                data.ErrorMessage = $"Неверное расширение изображения. Поддерживаемые расширения: ";
                foreach (var ex in _imageUploadSettings.AllowedExtensions)
                    data.ErrorMessage += $"{ex} ";
                return data;
            }
            if (size > _imageUploadSettings.MaxFileSize)
            {
                int megabytes = (int)(_imageUploadSettings.MaxFileSize / (1024 * 1024));

                data.Success = false;
                data.ErrorMessage = $"Слишком  большой размер изображения." +
                    $" Изображение должно быть до {megabytes} Mb.";
                return data;
            }
            return data;
        }

        protected async Task<ResponseData<Dictionary<string, string>>> SaveImage(IFormFile imageFormFile)
        {
            ResponseData<Dictionary<string, string>> saveImageData = new();

            ResponseData<string> validateImageData = await validateImage(imageFormFile);
            if (validateImageData.Success == false)
            {
                saveImageData.Data = null;
                saveImageData.Success = validateImageData.Success;
                saveImageData.ErrorMessage = validateImageData.ErrorMessage;
                return saveImageData;
            }

            Dictionary<string, string> data = new();
            using (Stream fileStreame = imageFormFile.OpenReadStream())
            {
                data = await _physicalFile.Save(fileStreame, imageFormFile.FileName, "Images");
            }

            string uri = await GetFileUri(data["relativePath"]);
            data.Add("uri", uri);
            saveImageData.Data = data;

            return saveImageData;
        }
        #endregion
    }
}
