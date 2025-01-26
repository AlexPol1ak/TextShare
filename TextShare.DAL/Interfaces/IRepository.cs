using System.Linq.Expressions;

namespace TextShare.DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {

        Task<IQueryable<TEntity>> GetAllAsync();
        Task<TEntity> GetAsync(int id, params string[] includes);
        Task<IQueryable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> Create(TEntity entity);
        Task<TEntity> Update(TEntity entity);
        Task<bool> Delete(int id);
        Task<bool> Contains(TEntity entity);
        Task<int> Count();

    }
}
