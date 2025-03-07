﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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

        public async Task<IQueryable<TextFile>> GetAllAsync(params Expression<Func<TextFile, object>>[] includes)
        {
            await Task.CompletedTask;
            IQueryable<TextFile> query = _textFiles.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public async Task<TextFile?> GetAsync(int id, params Expression<Func<TextFile, object>>[] includes)
        {
            IQueryable<TextFile> query = _textFiles.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(tf => tf.TextFileId == id);
        }

        public async Task<IQueryable<TextFile>> FindAsync(
                Expression<Func<TextFile, bool>> predicate,
                params Expression<Func<TextFile, object>>[] includes
            )
        {
            await Task.CompletedTask;
            IQueryable<TextFile> query = _textFiles.Where(predicate);

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
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
