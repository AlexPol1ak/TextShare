using System.Linq.Expressions;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Complaints;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.DAL.Repositories
{
    /// <summary>
    /// Класс доступа к данным.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TextShareContext _context;

        private IRepository<User> userRepository;
        private IRepository<Friendship> friendshipRepository;
        private IRepository<TextFile> textFileRepository;
        private IRepository<Shelf> shelfRepository;
        private IRepository<Category> categoryRepository; 
        private IRepository<Group> groupRepository;
        private IRepository<Complaint> complaintRepository;
        private IRepository<ComplaintReasons> complaintReasonsRepository;
        private IRepository<AccessRule> accessRuleRepository;

        public UnitOfWork(TextShareContext context)
        {
            _context = context;
        }

        // Репозиторий пользователя
        public IRepository<User> UserRepository => userRepository ??=
            new UserRepository(_context);

        // Репозиторий дружбы
        public IRepository<Friendship> FriendshipRepository=> friendshipRepository ??=
            new FriendshipRepository(_context);

        // Репозиторий текстовых файлов
        public IRepository<TextFile> TextFileRepository => textFileRepository ??=
            new TextFileRepository(_context);

        // Репозиторий полок
        public IRepository<Shelf> ShelfRepository => shelfRepository ??=
            new ShelfRepository(_context);

        // Репозиторий категорий
        public IRepository<Category> CategoryRepository => categoryRepository ??=
            new CategoryRepository(_context);

        // Репозиторий групп пользователей
        public IRepository<Group> GroupRepository => groupRepository ??=
            new GroupRepository(_context);

        // Репозиторий  жалоб
        public IRepository<Complaint> ComplaintRepository => complaintRepository ??=
            new ComplaintRepository(_context);

        // Репозиторий причин жалоб
        public IRepository<ComplaintReasons> ComplaintReasonsRepository =>
            complaintReasonsRepository ??= new ComplaintReasonsRepository(_context);

        // Репозиторий правил доступа
        public IRepository<AccessRule> AccessRuleRepository => accessRuleRepository ??=
            new AccessRuleRepository(_context);

        /// <summary>
        /// Сохраняет изменения в базе данных.
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Загружает связанные сущности.
        /// </summary>
        /// <typeparam name="T">Основной тип сущности.</typeparam>
        /// <typeparam name="TProperty">Тип зависимой сущности.</typeparam>
        /// <param name="entity"></param>
        /// <param name="navigationProperty">
        /// Выражение, указывающее на коллекцию связанных сущностей
        /// </param>
        public async Task LoadRelatedEntities<T, TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> navigationProperty)
                 where T : class
                 where TProperty : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (navigationProperty == null)
            {
                throw new ArgumentNullException(nameof(navigationProperty));
            }

            //_context.Entry(entity).Collection(navigationProperty).Load();
            await _context.Entry(entity).Collection(navigationProperty).LoadAsync();
        }
    }
}
