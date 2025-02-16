using System.Linq.Expressions;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Business.Interfaces
{

    /// <summary>
    ///  Интерфейс для сервиса управления полками
    /// </summary>
    public interface IShelfService : IBaseService
    {
        /// <summary>
        /// Возвращает все полки.
        /// </summary>
        /// <returns>Список полок</returns>
        Task<List<Shelf>> GetAllShelvesAsync(params Expression<Func<Shelf, object>>[] includes);

        /// <summary>
        /// Возвращает полку по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор полки</param>
        /// <returns>Возвращает полку, если найдено, иначе null</returns>
        Task<Shelf?> GetShelfByIdAsync(int id, params Expression<Func<Shelf, object>>[] includes);

        /// <summary>
        /// Поиск полки по условию
        /// </summary>
        /// <param name="predicate">Условие</param>
        /// <returns>Список полок</returns>
        Task<List<Shelf>> FindShelvesAsync(Expression<Func<Shelf, bool>> predicate, params Expression<Func<Shelf, object>>[] includes);

        /// <summary>
        /// Создает новую полку.ч
        /// </summary>
        /// <param name="shelf">Созданная полка</param>
        /// <returns></returns>
        Task<Shelf> CreateShelfAsync(Shelf shelf);

        /// <summary>
        ///  Удаляет полку по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор полки</param>
        /// <returns>true- если удалено успешно, иначе false</returns>
        Task<bool> DeleteShelfAsync(int id);

        /// <summary>
        /// Обновляет существующую полку.
        /// </summary>
        /// <param name="shelf">Полка для обновления</param>
        /// <returns>Обновленная полка</returns>
        Task<Shelf> UpdateShelfAsync(Shelf shelf);

        /// <summary>
        /// Проверяет, существует ли полка.
        /// Проверяет по идентификатору или по имени и дате создания.
        /// </summary>
        /// <param name="shelf">Полка для проверки</param>
        /// <returns>true- если существует, иначе false.</returns>
        Task<bool> ContainsShelfAsync(Shelf shelf);

        /// <summary>
        /// Возвращает все созданные полки  пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns>Полки пользователя</returns>
        Task<List<Shelf>> GetAllUserShelvesAsync(int userId, params Expression<Func<Shelf, object>>[] includes);

    }
}
