using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Domain.Models.EntityModels.ShelfModels
{
    /// <summary>
    /// Модель создания новой полки.
    /// </summary>
    public class ShelfCreateModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [StringLength(100, ErrorMessage = "Имя должно содержать не более 100 символов.")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я0-9\s!@#\$%\^&\*\(\)_\+\-=.,:;""'<>?{}[\]\\\/]*$",
            ErrorMessage = "Имя полки может содержать буквы, цифры, пробелы и спецсимволы.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Описание должно содержать не более 500 символов.")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я0-9\s!@#\$%\^&\*\(\)_\+\-=.,:;""'<>?{}[\]\\\/]*$",
            ErrorMessage = "Описание полки может содержать буквы, цифры, пробелы и спецсимволы.")]
        public string? Description { get; set; }

        /// <summary>
        /// Преобразует модель создания полки в сущность <see cref="Shelf"/>.
        /// </summary>
        /// <returns>Сущность полки.</returns>
        public Shelf ToShelf()
        {
            Shelf shelf = new();
            shelf.Name = Name;
            shelf.Description = Description;

            return shelf;
        }

        /// <summary>
        /// Преобразует сущность <see cref="Shelf"/> в модель создания полки.
        /// </summary>
        /// <param name="shelf">Сущность полки.</param>
        /// <returns>Модель создания полки.</returns>
        static public ShelfCreateModel FromShelf(Shelf shelf)
        {
            ShelfCreateModel shelfCreateModel = new();
            shelfCreateModel.Name = shelf.Name;
            shelfCreateModel.Description = shelf.Description;

            return shelfCreateModel;
        }
    }
}
