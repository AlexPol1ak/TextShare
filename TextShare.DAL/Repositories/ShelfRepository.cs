using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.DAL.Repositories
{
    /// <summary>
    /// Репозиторий полок.
    /// </summary>
    public class ShelfRepository : IRepository<Shelf>
    {
        private readonly DbSet<Shelf> _shelves;

        public ShelfRepository(TextShareContext context)
        {
            _shelves = context.Shelves;
        }

        public async Task<List<Shelf>> GetAllAsync(params string[] includes)
        {
            IQueryable<Shelf> query = _shelves.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public async Task<Shelf?> GetAsync(int id, params string[] includes)
        {
            IQueryable<Shelf> query = _shelves.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(s => s.ShelfId == id);
        }

        public async Task<List<Shelf>> FindAsync(Expression<Func<Shelf, bool>> predicate)
        {
            return await _shelves.Where(predicate).ToListAsync();
        }

        public async Task<Shelf> CreateAsync(Shelf entity)
        {
            var res = await _shelves.AddAsync(entity);
            return res.Entity;
        }

        public async Task<Shelf> UpdateAsync(Shelf entity)
        {
            var res = _shelves.Update(entity);
            await Task.CompletedTask;
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var shelf = await _shelves.FindAsync(id);
            if (shelf == null) return false;
            _shelves.Remove(shelf);
            return true;
        }

        public async Task<bool> ContainsAsync(Shelf entity)
        {
            return await _shelves.AnyAsync(s => s.ShelfId == entity.ShelfId ||
            (s.Name == entity.Name && s.CreatedAt ==entity.CreatedAt));
        }

        public async Task<int> CountAsync()
        {
            return await _shelves.CountAsync();
        }
    }
}
