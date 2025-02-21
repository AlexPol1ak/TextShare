using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;
using X.PagedList;

namespace TextShare.Domain.Models.EntityModels.FriendsModels
{
    /// <summary>
    /// Модель для передачи дружбы в представление
    /// </summary>
    public class FriendSearchResultModel
    {

        /// <summary>
        /// Авторизованный пользователь
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Друзья пользователя
        /// </summary>
        public IPagedList<User> Friends { get; set; }
        /// <summary>
        /// Входящие заявки на дружбу
        /// </summary>
        public IPagedList<User> InFriendRequests { get; set; }
        /// <summary>
        /// Исходящие заявки на дружбу
        /// </summary>
        public IPagedList<User> OutFriendRequests { get; set; }
        /// <summary>
        /// Поиск людей,если требуется.
        /// </summary>
        public IPagedList<User> ResultSearch { get; set; } 
    }
}
