using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _repositoryCategories;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _repositoryCategories = unitOfWork.CategoryRepository;
        }

        public async Task<bool> ContainsCategoryAsync(Category category)
        {
            return await _repositoryCategories.ContainsAsync(category);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            return await _repositoryCategories.CreateAsync(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
           return await _repositoryCategories.DeleteAsync(id);
        }

        public async Task<List<Category>> FindCategoriesAsync(Expression<Func<Category, bool>> predicate)
        {
            return await _repositoryCategories.FindAsync(predicate);
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _repositoryCategories.GetAllAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _repositoryCategories.GetAsync(id);
        }

        public async Task<Category?> GetCategoryByNameAsync(string categoryName)
        {
            List<Category> listCategory = await _repositoryCategories.
                FindAsync(c=>c.Name == categoryName);
            return listCategory.FirstOrDefault();

        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            return await _repositoryCategories.UpdateAsync(category);
        }
    }
}
