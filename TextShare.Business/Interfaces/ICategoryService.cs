using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Business.Interfaces
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetAllCategoriesAsync();
        public Task<Category?> GetCategoryByIdAsync(int id);
        public Task<List<Category>> FindCategoriesAsync(Expression<Func<Category, bool>> predicate);
        public Task<Category> CreateCategoryAsync(Category category);
        public Task<bool> DeleteCategoryAsync(int id);
        public Task<Category> UpdateCategoryAsync(Category category);
        public Task<bool> ContainsCategoryAsync(Category category);
        
    }
}
