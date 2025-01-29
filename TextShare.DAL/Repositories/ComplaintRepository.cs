using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.DAL.Repositories
{
    /// <summary>
    /// Репозиторий жалоб на файлы.
    /// </summary>
    public class ComplaintRepository : IRepository<Complaint>
    {
        private readonly DbSet<Complaint> _complaints;

        public ComplaintRepository(TextShareContext context)
        {
            _complaints = context.Complaints;
        }

        public async Task<List<Complaint>> GetAllAsync(params string[] includes)
        {
            IQueryable<Complaint> query = _complaints.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public async Task<Complaint?> GetAsync(int id, params string[] includes)
        {
            IQueryable<Complaint> query = _complaints.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(c => c.ComplaintId == id);
        }

        public async Task<List<Complaint>> FindAsync(Expression<Func<Complaint, bool>> predicate)
        {
            return await _complaints.Where(predicate).ToListAsync();
        }

        public async Task<Complaint> CreateAsync(Complaint entity)
        {
            var res = await _complaints.AddAsync(entity);
            return res.Entity;
        }

        public async Task<Complaint> UpdateAsync(Complaint entity)
        {
            var res = _complaints.Update(entity);
            await Task.CompletedTask;
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var complaint = await _complaints.FindAsync(id);
            if (complaint == null) return false;
            _complaints.Remove(complaint);
            return true;
        }

        public async Task<bool> ContainsAsync(Complaint entity)
        {
            return await _complaints.AnyAsync(c => c.TextFileId == entity.TextFileId && c.ComplaintReasonsId == entity.ComplaintReasonsId);
        }

        public async Task<int> CountAsync()
        {
            return await _complaints.CountAsync();
        }
    }
}
