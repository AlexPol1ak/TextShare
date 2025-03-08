using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Groups;

namespace TextShare.Domain.Models.EntityModels.GroupModels
{
    /// <summary>
    /// Модель обновления группы.
    /// </summary>
    public class GroupUpdateModel
    {
        public int GroupId { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [StringLength(100, ErrorMessage = "Имя должно содержать не более 100 символов.")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я0-9_#\-\s]+$",
            ErrorMessage = "Имя группы может содержать буквы (латинские или русские), цифры, пробелы, символы _ и #.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Описание должно содержать не более 500 символов.")]

        public string? ImageUri { get; set; }
        public string? Description { get; set; }

    }
}
