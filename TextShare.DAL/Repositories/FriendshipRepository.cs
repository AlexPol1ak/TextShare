using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Users;

namespace TextShare.DAL.Repositories
{
    /// <summary>
    /// Репозиторий дружбы.
    /// </summary>
    public class FriendshipRepository : IRepository<Friendship>
    {
        private readonly DbSet<Friendship> _friendships;

        public FriendshipRepository(TextShareContext context)
        {
            _friendships = context.Friendships;
        }

        public async Task<bool> ContainsAsync(Friendship entity)
        {
            return await _friendships.AnyAsync(f =>
                (f.UserId == entity.UserId && f.FriendId == entity.FriendId) ||
                (f.UserId == entity.FriendId && f.FriendId == entity.UserId));
        }

        public async Task<int> CountAsync()
        {
            return await _friendships.CountAsync();
        }

        public async Task<Friendship> CreateAsync(Friendship entity)
        {
            var res = await _friendships.AddAsync(entity);
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var friendship = await _friendships.FindAsync(id);
            if (friendship == null) return false;
            _friendships.Remove(friendship);
            return true;
        }

        public async Task<IQueryable<Friendship>> FindAsync(
            Expression<Func<Friendship, bool>> predicate,
            params Expression<Func<Friendship, object>>[] includes
            )
        {
            await Task.CompletedTask;
            IQueryable<Friendship> query = _friendships.Where(predicate);
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public async Task<IQueryable<Friendship>> GetAllAsync(params Expression<Func<Friendship, object>>[] includes)
        {
            await Task.CompletedTask;
            IQueryable<Friendship> query = _friendships.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public async Task<Friendship?> GetAsync(int id, params Expression<Func<Friendship, object>>[] includes)
        {
            IQueryable<Friendship> query = _friendships.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Friendship> UpdateAsync(Friendship entity)
        {
            var res = _friendships.Update(entity);
            await Task.CompletedTask;
            return res.Entity;
        }
    }
}
