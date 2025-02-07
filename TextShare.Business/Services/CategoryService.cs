using Microsoft.EntityFrameworkCore;
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

    /// <summary> 
    ///  Сервис для управления категориями
    /// </summary>
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly IRepository<Category> _repositoryCategories;

        public CategoryService(IUnitOfWork unitOfWork) : base(unitOfWork) 
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

        public async Task<List<Category>> GetAllCategoriesAsync(params string[] includes)
        {
            return await _repositoryCategories.GetAllAsync(includes);
        }

        public async Task<Category?> GetCategoryByIdAsync(int id, params string[] includes)
        {
            return await _repositoryCategories.GetAsync(id, includes);
        }

        public async Task<Category?> GetCategoryByNameAsync(string categoryName, params string[] includes)
        {
            IQueryable<Category> categories = (await _repositoryCategories.
                FindAsync(c=>c.Name == categoryName)).AsQueryable();
            if(includes.Length > 0)
            {
                foreach(string include in includes)
                {
                    categories.Include(include);
                }
            }
            List<Category> listCategory = categories.ToList();
            return listCategory.FirstOrDefault();

        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            return await _repositoryCategories.UpdateAsync(category);
        }
    }
}
