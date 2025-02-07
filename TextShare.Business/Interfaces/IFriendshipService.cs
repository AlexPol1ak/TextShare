using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс для сервиса управления дружбой пользователей.
    /// </summary>
    public interface IFriendshipService : IBaseService
    {
        /// <summary>
        /// Возвращает все записи о дружбе.
        /// </summary>
        /// <returns>Список всех дружеских связей</returns>
        Task<List<Friendship>> GetAllFriendshipsAsync(params string[] includes);

        /// <summary>
        /// Возвращает запись о дружбе по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор дружбы</param>
        /// <returns>Объект дружбы, если найден, иначе null</returns>
        Task<Friendship?> GetFriendshipByIdAsync(int id, params string[] includes);

        /// <summary>
        /// Возвращает список дружеских связей для конкретного пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Список дружеских связей</returns>
        Task<List<Friendship>> GetFriendshipsByUserIdAsync(int userId);

        /// <summary>
        /// Возвращает список дружеских связей, где пользователь указан как друг.
        /// </summary>
        /// <param name="friendId">Идентификатор друга</param>
        /// <returns>Список дружеских связей</returns>
        Task<List<Friendship>> GetFriendshipsByFriendIdAsync(int friendId);

        /// <summary>
        /// Создает новую запись о дружбе.
        /// </summary>
        /// <param name="friendship">Объект дружбы</param>
        /// <returns>Созданная запись о дружбе</returns>
        Task<Friendship> CreateFriendshipAsync(Friendship friendship);

        /// <summary>
        /// Удаляет запись о дружбе по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор дружбы</param>
        /// <returns>true, если удаление выполнено успешно, иначе false</returns>
        Task<bool> DeleteFriendshipAsync(int id);

        /// <summary>
        /// Обновляет существующую запись о дружбе.
        /// </summary>
        /// <param name="friendship">Объект дружбы</param>
        /// <returns>Обновленный объект дружбы</returns>
        Task<Friendship> UpdateFriendshipAsync(Friendship friendship);

        /// <summary>
        /// Проверяет, существует ли запись о дружбе.
        /// </summary>
        /// <param name="friendship">Объект дружбы</param>
        /// <returns>true, если запись существует, иначе false</returns>
        Task<bool> ContainsFriendshipAsync(Friendship friendship);

        /// <summary>
        /// Подтверждает дружбу по идентификатору.
        /// </summary>
        /// <param name="friendshipId">Идентификатор дружбы</param>
        /// <returns>true, если подтверждение успешно, иначе false</returns>
        Task<bool> ConfirmFriendshipAsync(int friendshipId);

        /// <summary>
        /// Отклоняет запрос на дружбу по идентификатору.
        /// </summary>
        /// <param name="friendshipId">Идентификатор дружбы</param>
        /// <returns>true, если отклонение успешно, иначе false</returns>
        Task<bool> RejectFriendshipAsync(int friendshipId);
    }
}
