using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Domain.Models.EntityModels
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Модель категории.
        /// </summary>
        public virtual Category ToCategory()
        {
            Category category = new Category()
            {
                Name = Name,
                Description = Description,
                CategoryId = CategoryId
            };
            return category;
        }

        // <summary>
        /// Преобразует сущность <see cref="Category"/> в модель категории.
        /// </summary>
        /// <param name="category">Сущность категории.</param>
        /// <returns>Модель категории.</returns>
        public static CategoryModel FromCategory(Category category)
        {
            CategoryModel dto = new CategoryModel();
            dto.CategoryId = category.CategoryId;
            dto.Name = category.Name;
            dto.Description = category.Description;
            return dto;
        }
    }
}
