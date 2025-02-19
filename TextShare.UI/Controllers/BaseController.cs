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
        /// <summary>
        /// Формирует полный URL к файлу на основе относительного пути.
        /// </summary>
        /// <param name="relativePath">Относительный путь к файлу.</param>
        /// <returns>Строка с полным URL к файлу.</returns>
        protected async Task<string> GetFileUri(string relativePath)
        {
            await Task.CompletedTask;
            string normalizedPath = relativePath.Replace('\\', '/').TrimStart('/');
            string fullUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{normalizedPath}";
            return fullUrl;
        }

        /// <summary>
        /// Проверяет корректность загруженного изображения по типу, расширению и размеру.
        /// </summary>
        /// <param name="imageFormFile">Загруженный файл.</param>
        /// <returns>
        /// Объект ResponseData, содержащий флаг успешности и сообщение об ошибке, если изображение не соответствует требованиям.
        /// </returns>
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

        /// <summary>
        /// Сохраняет изображение на сервере после проверки.
        /// </summary>
        /// <param name="imageFormFile">Файл изображения для сохранения.</param>
        /// <returns>
        /// Объект <see cref="ResponseData{Dictionary{string, string}}"/> с информацией о сохраненном файле.  
        /// 
        /// В случае успеха `Data` содержит следующий словарь:
        /// <code>
        /// {
        ///     "originalFileName": "Исходное имя файла",
        ///     "uniqueFileName": "Уникальное имя файла",
        ///     "type": "Расширение файла",
        ///     "size": "Размер файла в байтах",
        ///     "relativePath": "Относительный путь к файлу",
        ///     "uri": "Полный URL файла"
        /// }
        /// </code>
        /// 
        /// В случае ошибки валидации возвращает `Success = false` и `ErrorMessage`.
        /// </returns>
        protected async Task<ResponseData<Dictionary<string, string>>> SaveImage(IFormFile imageFormFile)
        {
            ResponseData<string> validateImageData = await validateImage(imageFormFile);
            ResponseData<Dictionary<string, string>> saveImageData = new();
       
            if (validateImageData.Success == false)
            {
                saveImageData.Data = null;
                saveImageData.Success = validateImageData.Success;
                saveImageData.ErrorMessage = validateImageData.ErrorMessage;
                return saveImageData;
            }

            Dictionary<string, string> dataDict = new();
            try
            {
                using (Stream fileStreame = imageFormFile.OpenReadStream())
                {
                    dataDict = await _physicalFile.Save(fileStreame, imageFormFile.FileName, "Images");
                }
            }
            catch
            {
                saveImageData.Success = false;
                saveImageData.ErrorMessage = "Не удалось сохранить изображение.";
                return saveImageData;
            }

            string uri = await GetFileUri(dataDict["relativePath"]);
            dataDict.Add("uri", uri);
            saveImageData.Data = dataDict;

            return saveImageData;
        }

        /// <summary>
        /// Удаляет физичекий файл изображения по Uri
        /// </summary>
        /// <param name="uriImage">Uri файла</param>
        /// <returns></returns>
        protected async Task<bool> DeleteImageByUri(string uriImage)
        {
            string fileName = string.Empty;
            try
            {
                fileName = Path.GetFileName(new Uri(uriImage).LocalPath);
            }
            catch
            {
                fileName = Path.GetFileName(uriImage);
            }
            if (string.IsNullOrEmpty(fileName)) return false;

            return await _physicalFile.Delete(fileName, "Images");
        }
        #endregion
    }
}
