using System.Linq.Expressions;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс для сервиса управления пользователями.
    /// </summary>
    public interface IUserService : IBaseService
    {
        /// <summary>
        /// Возвращает всех пользователей.
        /// </summary>
        /// <returns>Список пользователей</returns>
        Task<List<User>> GetAllUsersAsync(params Expression<Func<User, object>>[] includes);

        /// <summary>
        /// Возвращает пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Возвращает пользователя, если найден, иначе null</returns>
        Task<User?> GetUserByIdAsync(int id, params Expression<Func<User, object>>[] includes);

        /// <summary>
        /// Возвращает пользователя по имени пользователя (логину).
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <returns>Возвращает пользователя, если найден, иначе null</returns>
        Task<User?> GetUserByUsernameAsync(string username, params Expression<Func<User, object>>[] includes);

        /// <summary>
        /// Поиск пользователей по условию.
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        /// <returns>Список найденных пользователей</returns>
        Task<List<User>> FindUsersAsync(Expression<Func<User, bool>> predicate);

        /// <summary>
        /// Создает нового пользователя.
        /// </summary>
        /// <param name="user">Создаваемый пользователь</param>
        /// <returns>Созданный пользователь</returns>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Удаляет пользователя по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>true, если удаление выполнено успешно, иначе false</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Обновляет существующего пользователя.
        /// </summary>
        /// <param name="user">Пользователь для обновления</param>
        /// <returns>Обновленный пользователь</returns>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// Проверяет, существует ли пользователь.
        /// </summary>
        /// <param name="user">Пользователь для проверки</param>
        /// <returns>true, если существует, иначе false</returns>
        Task<bool> ContainsUserAsync(User user);
    }
}
