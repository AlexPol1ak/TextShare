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
    /// Репозиторий текстовых файлов.
    /// </summary>
    public class TextFileRepository : IRepository<TextFile>
    {
        private readonly DbSet<TextFile> _textFiles;

        public TextFileRepository(TextShareContext context)
        {
            _textFiles = context.TextFiles;
        }

        public async Task<List<TextFile>> GetAllAsync(params string[] includes)
        {
            IQueryable<TextFile> query = _textFiles.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public async Task<TextFile?> GetAsync(int id, params string[] includes)
        {
            IQueryable<TextFile> query = _textFiles.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(tf => tf.TextFileId == id);
        }

        public async Task<List<TextFile>> FindAsync(Expression<Func<TextFile, bool>> predicate)
        {
            return await _textFiles.Where(predicate).ToListAsync();
        }

        public async Task<TextFile> CreateAsync(TextFile entity)
        {
            var res = await _textFiles.AddAsync(entity);
            return res.Entity;
        }

        public async Task<TextFile> UpdateAsync(TextFile entity)
        {
            var res = _textFiles.Update(entity);
            await Task.CompletedTask;
            return res.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var textFile = await _textFiles.FindAsync(id);
            if (textFile == null) return false;
            _textFiles.Remove(textFile);
            return true;
        }

        public async Task<bool> ContainsAsync(TextFile entity)
        {
            return await _textFiles.AnyAsync(tf => tf.UniqueFileName == entity.UniqueFileName);
        }

        public async Task<int> CountAsync()
        {
            return await _textFiles.CountAsync();
        }
    }
}
