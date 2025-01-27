using System.Linq.Expressions;

namespace TextShare.DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {

        Task<List<TEntity>> GetAllAsync(params string[] includes);
        Task<TEntity?> GetAsync(int id, params string[] includes);
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ContainsAsync(TEntity entity);
        Task<int> CountAsync();

    }
}
