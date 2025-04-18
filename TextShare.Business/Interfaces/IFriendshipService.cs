﻿using System.Linq.Expressions;
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
        Task<List<Friendship>> GetAllFriendshipsAsync(params Expression<Func<Friendship, object>>[] includes);

        /// <summary>
        /// Возвращает запись о дружбе по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор дружбы</param>
        /// <returns>Объект дружбы, если найден, иначе null</returns>
        Task<Friendship?> GetFriendshipByIdAsync(int id, params Expression<Func<Friendship, object>>[] includes);

        /// <summary>
        /// Возвращает всех подтвержденных друзей для конкретного пользователя.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<List<Friendship>> GetAllUserAcceptedFriendshipAsync(int userId, params Expression<Func<Friendship, object>>[] includes);

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

        /// <summary>
        /// Поиск дружбы по условию
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<List<Friendship>> FindFriendshipsAsync(Expression<Func<
            Friendship, bool>> predicate,
            params Expression<Func<Friendship, object>>[] includes);

        /// <summary>
        /// Возвращает пользователей, которым отправлены запросы в друзья.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<User>> GetOutFriendRequestsUsers(int userId,
            params Expression<Func<User, object>>[] includes);

        /// <summary>
        /// Возвращает пользователей, которые отправили запросы в друзья.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<User>> GetInFriendRequestsUsers(int userId,
            params Expression<Func<User, object>>[] includes);

        /// <summary>
        /// Возвращает список друзей пользователя.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<List<User>> GetFriendsUser(int userId,
            params Expression<Func<User, object>>[] includes);
    }
}
