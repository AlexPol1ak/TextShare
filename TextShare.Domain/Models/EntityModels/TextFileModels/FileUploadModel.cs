using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Domain.Models
{
    public class FileUploadModel
    {
        [Required(ErrorMessage = "Описание файла обязательно.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Необходимо выбрать хотя бы одну категорию.")]
        public List<int> SelectedCategoryIds { get; set; } = new();

        public List<Category> Categories { get; set; } = new();
    }
}
