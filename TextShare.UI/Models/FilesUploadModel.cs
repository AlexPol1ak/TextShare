using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Models;

namespace TextShare.UI.Models
{
    public class FilesUploadModel : FileUploadModel
    {

        [Required(ErrorMessage = "Выберите файл.")]
        public IFormFile File { get; set; }
    }
}
