using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextShare.Domain.Entities.Users
{
    /// <summary>
    /// Класс дружбы.
    /// </summary>
    public class Friendship
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int FriendId { get; set; }
        public User Friend { get; set; }

        public DateTime CreatedAd { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Если дружба подтверждена -True. Если в заявке False.
        /// Удалить запись -если отклонена
        /// </summary>
        public bool IsConfirmed { get; set; } = false;

        public override string ToString()
        {
            return $"Id: {Id}. User: {User.ToString()}. Friend: {Friend.ToString()}. " +
                $"Created At: {CreatedAd.ToShortTimeString()}. Is Confirmed {IsConfirmed.ToString()}";
        }

    }
}
