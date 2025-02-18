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
        [StringLength(100, ErrorMessage = "Имя должно содержать не более 45 символов.")]
        public string Name { get; set; }
        [StringLength(100, ErrorMessage = "Описание должно содержать не более 500 символов.")]
        public string? Description { get; set; }

        public Shelf ToShelf()
        {
            Shelf shelf = new();
            shelf.Name = Name;
            shelf.Description = Description;

            return shelf;

        }
    }

    
}
