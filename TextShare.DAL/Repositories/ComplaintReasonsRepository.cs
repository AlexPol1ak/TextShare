using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.DAL.Repositories
{
    /// <summary>
    /// Репозиторий причин жалоб.
    /// </summary>
    public class ComplaintReasonsRepository : IRepository<ComplaintReasons>
    {
        private readonly DbSet<ComplaintReasons> _complaintReasons;

        public ComplaintReasonsRepository(TextShareContext context)
        {
            _complaintReasons = context.ComplaintReasons;
        }

        public async Task<IQueryable<ComplaintReasons>> GetAllAsync(params Expression<Func<ComplaintReasons, object>>[] includes)
        {
            await Task.CompletedTask;
            IQueryable<ComplaintReasons> query = _complaintReasons.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public async Task<ComplaintReasons?> GetAsync(int id, params Expression<Func<ComplaintReasons, object>>[] includes)
        {
            IQueryable<ComplaintReasons> query = _complaintReasons.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(c => c.ComplaintReasonsId == id);
        }

        public async Task<IQueryable<ComplaintReasons>> FindAsync(
                    Expression<Func<ComplaintReasons, bool>> predicate,
                    params Expression<Func<ComplaintReasons, object>>[] includes
            )
        {
            await Task.CompletedTask;
            IQueryable<ComplaintReasons> query = _complaintReasons.Where(predicate);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public async Task<ComplaintReasons> CreateAsync(ComplaintReasons entity)
        {
            var res = await _complaintReasons.AddAsync(entity);
            return res.Entity;
        }

        public async Task<ComplaintReasons> UpdateAsync(ComplaintReasons entity)
        {
            var res = _complaintReasons.Update(entity);
            await Task.CompletedTask;
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var complaintReason = await _complaintReasons.FindAsync(id);
            if (complaintReason == null) return false;
            _complaintReasons.Remove(complaintReason);
            return true;
        }

        public async Task<bool> ContainsAsync(ComplaintReasons entity)
        {
            return await _complaintReasons.AnyAsync(c => c.Name == entity.Name);
        }

        public async Task<int> CountAsync()
        {
            return await _complaintReasons.CountAsync();
        }
    }
}
