using System.ComponentModel.DataAnnotations;

namespace TextShare.Domain.Models.EntityModels.ShelfModels
{
    /// <summary>
    /// Модель поиска полок.
    /// </summary>
    public class ShelvesSearchModel
    {
        [StringLength(45, ErrorMessage = "Имя должно содержать не более 45 символов.")]
        public string? Name { get; set; }
        [StringLength(200, ErrorMessage = "Описание должно содержать не более 200 символов.")]
        public string? Description { get; set; }

        public bool OnlyAvailableMe { get; set; } = false;
    }
}
