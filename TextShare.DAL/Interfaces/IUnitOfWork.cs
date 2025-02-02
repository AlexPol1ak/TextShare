using System.Linq.Expressions;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Complaints;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;


namespace TextShare.DAL.Interfaces
{
    /// <summary>
    /// Интерфейс единицы работы
    /// </summary>
    public interface IUnitOfWork
    {
        IRepository<User> UserRepository { get; }
        IRepository<Friendship> FriendshipRepository { get; }
        IRepository<TextFile> TextFileRepository { get; }
        IRepository<Shelf> ShelfRepository { get; }
        IRepository<Category> CategoryRepository { get; }
        IRepository<Group> GroupRepository { get; }
        IRepository<Complaint> ComplaintRepository { get; }
        IRepository<ComplaintReasons> ComplaintReasonsRepository { get; }
        IRepository<AccessRule> AccessRuleRepository { get; }

        public Task<int> SaveChangesAsync();

        public Task LoadRelatedEntities<T, TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> navigationProperty)
          where T : class
          where TProperty : class;
    }
}
