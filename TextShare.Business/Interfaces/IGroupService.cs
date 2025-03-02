using System.Linq.Expressions;
using TextShare.Domain.Entities.Groups;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс для сервиса управления группами.
    /// </summary>
    public interface IGroupService : IBaseService
    {
        /// <summary>
        /// Возвращает список всех групп.
        /// </summary>
        /// <returns>Список групп</returns>
        Task<List<Group>> GetAllGroupsAsync(params Expression<Func<Group, object>>[] includes);

        /// <summary>
        /// Возвращает группу по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор группы</param>
        /// <returns>Объект группы, если найден, иначе null</returns>
        Task<Group?> GetGroupByIdAsync(int id, params Expression<Func<Group, object>>[] includes);

        /// <summary>
        /// Поиск групп по указанному условию.
        /// </summary>
        /// <param name="predicate">Лямбда-выражение с условием поиска</param>
        /// <returns>Список групп, удовлетворяющих условию</returns>
        Task<List<Group>> FindGroupsAsync(Expression<Func<Group, bool>> predicate, 
            params Expression<Func<Group, object>>[] includes
            );

        /// <summary>
        /// Создает новую группу.
        /// </summary>
        /// <param name="group">Объект группы</param>
        /// <returns>Созданная группа</returns>
        Task<Group> CreateGroupAsync(Group group);

        /// <summary>
        /// Удаляет группу по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор группы</param>
        /// <returns>true, если удаление выполнено успешно, иначе false</returns>
        Task<bool> DeleteGroupAsync(int id);

        /// <summary>
        /// Обновляет данные группы.
        /// </summary>
        /// <param name="group">Обновленный объект группы</param>
        /// <returns>Обновленный объект группы</returns>
        Task<Group> UpdateGroupAsync(Group group);

        /// <summary>
        /// Проверяет существование группы в системе.
        /// </summary>
        /// <param name="group">Объект группы</param>
        /// <returns>true, если группа существует, иначе false</returns>
        Task<bool> ContainsGroupAsync(Group group);

        /// <summary>
        /// Получает все группы созданные пользователем.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="includes"></param>
        /// <returns>Список групп, которые создал пользователь</returns>
        Task<List<Group>> GetUserCreatedGroupsAsync(int userId,
            params Expression<Func<Group, object>>[] includes);
    }
}
