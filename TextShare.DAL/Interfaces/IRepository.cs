using System.Linq.Expressions;

namespace TextShare.DAL.Interfaces
{
    /// <summary>
    /// Универсальный репозиторий для работы с сущностями в базе данных.
    /// </summary>
    public interface IRepository<TEntity> where TEntity : class
    {

        Task<IQueryable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> GetAsync(int id, params Expression<Func<TEntity, object>>[] includes);
        Task<IQueryable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes
            );
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ContainsAsync(TEntity entity);
        Task<int> CountAsync();

    }
}
