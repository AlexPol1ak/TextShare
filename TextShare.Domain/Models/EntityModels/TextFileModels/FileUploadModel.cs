using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Domain.Models
{
    /// <summary>
    /// Модель загрузки файла.
    /// </summary>
    public class FileUploadModel
    {
        [Required(ErrorMessage = "Введите описание файла.")]
        [StringLength(500, ErrorMessage = "Описание не может превышать 500 символов.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Выберите хотя бы одну категорию.")]
        public List<int> SelectedCategoryIds { get; set; } = new();

        public List<Category> Categories { get; set; } = new();
    }
}
