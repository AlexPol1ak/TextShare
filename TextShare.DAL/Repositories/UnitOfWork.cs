using System.Linq.Expressions;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly TextShareContext _context;

        private IRepository<User> userRepository;
        private IRepository<Friendship> friendshipRepository;
        private IRepository<TextFile> textFileRepository;

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
