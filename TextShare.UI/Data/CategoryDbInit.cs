

using System.Text.Json;
using TextShare.Business.Interfaces;
using TextShare.Business.Services;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.UI.Data
{
    /// <summary>
    /// Класс  инициализации категорий в базе данных.
    /// </summary>
    public class CategoryDbInit : DbInitDataAbstract
    {
        private readonly ICategoryService categoryService;
        public bool InstallCategories { get; set; } = true;
        public CategoryDbInit(WebApplication webApp) : base(webApp)
        {
            categoryService = this.scope.ServiceProvider.GetRequiredService<ICategoryService>();
        }

        public override async Task<bool> SeedData()
        {
            if (!InstallCategories) return false;

            int categoryNumber = await installCategories();
            return categoryNumber > 0;

        }

        public async Task<int> installCategories()
        {
            int counter = 0;

            List<Category> categories = await  createCategories();

            foreach(Category cat in categories)
            {
                if(!(await categoryService.ContainsCategoryAsync(cat)))
                {
                    await categoryService.CreateCategoryAsync(cat);
                    await categoryService.SaveAsync();
                    counter++;
                }
            }                   
            return counter;
        }

        private async Task<List<Category>> createCategories()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "categories.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл {filePath} не найден.");
            }

            string json = await File.ReadAllTextAsync(filePath);
            var categories = JsonSerializer.Deserialize<List<Category>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return categories ?? new List<Category>();
        }
        
    }
}
