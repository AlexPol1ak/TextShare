using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.DAL.Repositories
{
    /// <summary>
    /// Репозиторий категорий.
    /// </summary>
    public class CategoryRepository : IRepository<Category>
    {
        private readonly DbSet<Category> _categories;

        public CategoryRepository(TextShareContext context)
        {
            _categories = context.Categories;
        }

        public async Task<List<Category>> GetAllAsync(params Expression<Func<Category, object>>[] includes)
        {
            IQueryable<Category> query = _categories.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public async Task<Category?> GetAsync(int id, params Expression<Func<Category, object>>[] includes)
        {
            IQueryable<Category> query = _categories.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<List<Category>> FindAsync(
                    Expression<Func<Category, bool>> predicate,
                    params Expression<Func<Category, object>>[] includes
            )
        {
            IQueryable<Category> query = _categories.Where(predicate);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<Category> CreateAsync(Category entity)
        {
            var res = await _categories.AddAsync(entity);
            return res.Entity;
        }

        public async Task<Category> UpdateAsync(Category entity)
        {
            var res = _categories.Update(entity);
            await Task.CompletedTask;
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categories.FindAsync(id);
            if (category == null) return false;
            _categories.Remove(category);
            return true;
        }

        public async Task<bool> ContainsAsync(Category entity)
        {
            return await _categories.AnyAsync(c => c.Name == entity.Name);
        }

        public async Task<int> CountAsync()
        {
            return await _categories.CountAsync();
        }
    }
}
