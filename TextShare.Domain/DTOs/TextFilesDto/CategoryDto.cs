using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Domain.DTOs.TextFilesDto
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public virtual Category ToCategory()
        {
            Category category = new Category() {
                Name = Name,
                Description = Description,
                CategoryId = CategoryId
            };
            return category;
        }

        public static CategoryDto FromCategory(Category category)
        {
            CategoryDto dto = new CategoryDto();
            dto.CategoryId = category.CategoryId;
            dto.Name = category.Name;
            dto.Description = category.Description;
            return dto;
        }
    }
}
