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
        /// Асинхронно загружает коллекцию связанных сущностей для переданной основной сущности.
        /// Используется в случаях, когда ленивая загрузка (Lazy Loading) отключена и необходимо явно загружать зависимости.
        /// </summary>
        /// <typeparam name="T">Тип основной сущности.</typeparam>
        /// <typeparam name="TProperty">Тип зависимой сущности (коллекции).</typeparam>
        /// <param name="entity">Экземпляр основной сущности, для которой требуется загрузить связанные данные.</param>
        /// <param name="navigationProperty">
        /// Лямбда-выражение, указывающее на коллекцию связанных сущностей (например, x => x.Comments).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если переданная сущность (entity) или выражение навигационного свойства (navigationProperty) равно null.
        /// </exception>
        /// <remarks>
        /// Этот метод полезен, когда необходимо явно загружать коллекции зависимых данных, 
        /// вместо использования Include() в LINQ-запросах или если требуется подгрузить данные после получения основной сущности.
        /// </remarks>
        /// <example>
        /// Пример использования метода:
        /// <code>
        /// var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == 1);
        /// await LoadRelatedEntities(post, p => p.Comments);
        /// </code>
        /// После выполнения метода у объекта post будет загружена коллекция Comments.
        /// </example>
        public async Task LoadRelatedEntitiesAsync<T, TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> navigationProperty)
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
