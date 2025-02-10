using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Groups;

namespace TextShare.DAL.Repositories
{
    /// <summary>
    /// Репозиторий групп пользователей.
    /// </summary>
    public class GroupRepository : IRepository<Group>
    {
        private readonly DbSet<Group> _groups;

        public GroupRepository(TextShareContext context)
        {
            _groups = context.Groups;
        }

        public async Task<List<Group>> GetAllAsync(params Expression<Func<Group, object>>[] includes)
        {
            IQueryable<Group> query = _groups.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public async Task<Group?> GetAsync(int id, params Expression<Func<Group, object>>[] includes)
        {
            IQueryable<Group> query = _groups.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(g => g.GroupId == id);
        }

        public async Task<List<Group>> FindAsync(Expression<Func<Group, bool>> predicate)
        {
            return await _groups.Where(predicate).ToListAsync();
        }

        public async Task<Group> CreateAsync(Group entity)
        {
            var res = await _groups.AddAsync(entity);
            return res.Entity;
        }

        public async Task<Group> UpdateAsync(Group entity)
        {
            var res = _groups.Update(entity);
            await Task.CompletedTask;
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var group = await _groups.FindAsync(id);
            if (group == null) return false;
            _groups.Remove(group);
            return true;
        }

        public async Task<bool> ContainsAsync(Group entity)
        {
            return await _groups.AnyAsync(g => g.Name == entity.Name &&
            g.Description == entity.Description &&
            g.CreatedAt == entity.CreatedAt);
        }

        public async Task<int> CountAsync()
        {
            return await _groups.CountAsync();
        }
    }
}
