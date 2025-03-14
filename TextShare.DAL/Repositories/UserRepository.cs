﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Users;

namespace TextShare.DAL.Repositories
{
    /// <summary>
    /// Репозиторий пользователя.
    /// </summary>
    public class UserRepository : IRepository<User>
    {

        private readonly DbSet<User> _users;

        public UserRepository(TextShareContext context)
        {
            _users = context.Users;
        }

        public async Task<bool> ContainsAsync(User entity)
        {
            return await _users.AnyAsync(u => u.NormalizedEmail == entity.NormalizedEmail);
        }

        public async Task<int> CountAsync()
        {
            return await _users.CountAsync();
        }

        public async Task<User> CreateAsync(User entity)
        {
            var res = await _users.AddAsync(entity);
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _users.FindAsync(id);
            if (user == null) return false;
            _users.Remove(user);
            return true;
        }

        public async Task<IQueryable<User>> FindAsync(
                    Expression<Func<User, bool>> predicate,
                    params Expression<Func<User, object>>[] includes
            )
        {
            await Task.CompletedTask;
            IQueryable<User> query = _users.Where(predicate);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task<IQueryable<User>> GetAllAsync(params Expression<Func<User, object>>[] includes)
        {
            await Task.CompletedTask;
            IQueryable<User> query = _users.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public async Task<User?> GetAsync(int id, params Expression<Func<User, object>>[] includes)
        {
            IQueryable<User> query = _users.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            var user = await query.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<User> UpdateAsync(User entity)
        {
            var res = _users.Update(entity);
            await Task.CompletedTask;
            return res.Entity;
        }
    }
}
