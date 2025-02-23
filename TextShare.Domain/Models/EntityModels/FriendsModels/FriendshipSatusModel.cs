using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Models.EntityModels.FriendsModels
{
    /// <summary>
    /// Перечисление статусов дружбы
    /// </summary>
    public enum FriendStatus
    {
        [Display(Name = "Не друзья")]
        None = 0,

        [Display(Name = "Заявка отправлена")]
        Requested = 1,

        [Display(Name = "Заявка получена")]
        Pending = 2,

        [Display(Name = "Друзья")]
        Accepted = 3,
        [Display(Name = "Я")]
        Iam = 4



    }

    /// <summary>
    /// Модель дружбы пользователя.
    /// </summary>
    public class FriendshipSatusModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string AvatarUri { get; set; }

        public FriendStatus FriendStatus { get; set; }


        public static async Task<FriendshipSatusModel> FromUser(User user)
        {
            await Task.CompletedTask;
            FriendshipSatusModel friendshipSatusModel = new();
            friendshipSatusModel.Id = user.Id;
            friendshipSatusModel.UserName = user.UserName;
            friendshipSatusModel.FirstName = user.FirstName;
            friendshipSatusModel.LastName = user.LastName;
            friendshipSatusModel.AvatarUri = user.AvatarUri;
            return friendshipSatusModel;
        }

        public static async Task<IEnumerable<FriendshipSatusModel>> FromUsers(IEnumerable<User> users)
        {
            List<FriendshipSatusModel> friendshipSatusModels = new();

            var tasks = users.Select(async user => await FromUser(user));
            friendshipSatusModels =  (await Task.WhenAll(tasks)).ToList();
            return friendshipSatusModels;


        }


    }
}
