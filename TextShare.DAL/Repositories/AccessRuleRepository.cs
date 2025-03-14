﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.AccessRules;

namespace TextShare.DAL.Repositories
{
    /// <summary>
    /// Репозиторий правил доступа.
    /// </summary>
    public class AccessRuleRepository : IRepository<AccessRule>
    {
        private readonly DbSet<AccessRule> _accessRules;

        public AccessRuleRepository(TextShareContext context)
        {
            _accessRules = context.AccessRules;
        }

        public async Task<IQueryable<AccessRule>> GetAllAsync(params Expression<Func<AccessRule, object>>[] includes)
        {
            await Task.CompletedTask;
            IQueryable<AccessRule> query = _accessRules.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public async Task<AccessRule?> GetAsync(int id, params Expression<Func<AccessRule, object>>[] includes)
        {
            IQueryable<AccessRule> query = _accessRules.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(ar => ar.AccessRuleId == id);
        }

        public async Task<IQueryable<AccessRule>> FindAsync(
                    Expression<Func<AccessRule, bool>> predicate,
                    params Expression<Func<AccessRule, object>>[] includes
            )
        {
            await Task.CompletedTask;
            IQueryable<AccessRule> query = _accessRules.Where(predicate);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }


        public async Task<AccessRule> CreateAsync(AccessRule entity)
        {
            var res = await _accessRules.AddAsync(entity);
            return res.Entity;
        }

        public async Task<AccessRule> UpdateAsync(AccessRule entity)
        {
            var res = _accessRules.Update(entity);
            await Task.CompletedTask;
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var accessRule = await _accessRules.FindAsync(id);
            if (accessRule == null) return false;
            _accessRules.Remove(accessRule);
            return true;
        }

        public async Task<bool> ContainsAsync(AccessRule entity)
        {
            return await _accessRules.AnyAsync(ar => ar.TextFileId == entity.TextFileId);
        }

        public async Task<int> CountAsync()
        {
            return await _accessRules.CountAsync();
        }
    }
}
