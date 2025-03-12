using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.ShelfModels;

namespace TextShare.UI.Models
{
    /// <summary>
    /// Модель загрузки файлов.
    /// </summary>
    public class FilesUploadModel : FileUploadModel
    {

        [Required(ErrorMessage = "Выберите файл.")]
        public IFormFile File { get; set; }

        public int ShelfId { get; set; }
        public string ShelfName { get; set; }
        public string AllowedExtensionsStr { get; set; }
    }
}
