using System.Linq.Expressions;
using TextShare.Domain.Entities.Users;


namespace TextShare.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс единицы работы
    /// </summary>
    public interface IUnitOfWork
    {
        IRepository<User> UserRepository { get; }

        public Task<int> SaveChangesAsync();

        public Task LoadRelatedEntities<T, TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> navigationProperty)
          where T : class
          where TProperty : class;
    }
}
