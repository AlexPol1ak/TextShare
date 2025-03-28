
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Groups;

namespace TextShare.Domain.Models.EntityModels.GroupModels
{
    /// <summary>
    /// Модель для создания группы.
    /// </summary>
    public class GroupCreateModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [StringLength(100, ErrorMessage = "Имя должно содержать не более 100 символов.")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я0-9_#\-\s]+$",
            ErrorMessage = "Имя группы может содержать буквы (латинские или русские), цифры, пробелы, символы _ и #.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Описание должно содержать не более 500 символов.")]
        public string? Description { get; set; }

        /// <summary>
        /// Преобразует модель в сущность <see cref="Group"/>.
        /// </summary>
        /// <returns>Экземпляр <see cref="Group"/>.</returns>
        public Group ToGroup()
        {
            return new Group
            {
                Name = Name,
                Description = Description
            };
        }
    }
}
