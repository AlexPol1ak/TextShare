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
        [RegularExpression(@"^[a-zA-Zа-яА-Я\s]+$",
            ErrorMessage = "Имя полки  может содержать только буквы (латинские или русские) и пробелы.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Описание должно содержать не более 500 символов.")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я\s]+$",
            ErrorMessage = "Описание полки  может содержать только буквы (латинские или русские) и пробелы.")]
        public string? Description { get; set; }

        public Shelf ToShelf()
        {
            Shelf shelf = new();
            shelf.Name = Name;
            shelf.Description = Description;

            return shelf;
        }

        static public ShelfCreateModel FromShelf(Shelf shelf)
        {
            ShelfCreateModel shelfCreateModel = new();
            shelfCreateModel.Name = shelf.Name;
            shelfCreateModel.Description = shelf.Description;

            return shelfCreateModel;
        }
    }

    
}
