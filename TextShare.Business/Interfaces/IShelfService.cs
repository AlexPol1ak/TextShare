using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

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
        Task<List<Shelf>> GetAllShelvesAsync();

        /// <summary>
        /// Возвращает полку по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор полки</param>
        /// <returns>Возвращает полку, если найдено, иначе null</returns>
        Task<Shelf?> GetShelfByIdAsync(int id);

        /// <summary>
        /// Поиск полки по условию
        /// </summary>
        /// <param name="predicate">Условие</param>
        /// <returns>Список полок</returns>
        Task<List<Shelf>> FindShelvesAsync(Expression<Func<Shelf, bool>> predicate);

        /// <summary>
        /// Создает новую полку.ч
        /// </summary>
        /// <param name="shelf">Созданная полка</param>
        /// <returns></returns>
        Task<Shelf> CreateShelfAsync(Shelf shelf);
        
        /// <summary>
        /// Добавляет полке владельца.
        /// Если полка существует - обновляет полку, если не существует- создает новую.
        /// </summary>
        /// <param name="shelf">Полка</param>
        /// <param name="user">Владелец</param>
        /// <returns>Полка с установленным владельцем</returns>
        Task<Shelf> AddCreatorShelfAsync(Shelf shelf, User user);

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
    }
}
