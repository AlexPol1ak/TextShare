using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models.EntityModels.FriendsModels;

namespace TextShare.Domain.Models.EntityModels.UserModels
{
    public class UserDetailModel :UserModel
    {
        public int CountAvailableShelves { get; set; } 
        public int CountAvailableTextFiles { get; set; }
        public int CountFriends { get; set; } 

        public FriendStatus? RelationshipTocurrentUser { get; set; } = null;


        /// <summary>
        /// Создает модель из объекта User.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public new static UserDetailModel FromUser(User user)
        {
            UserDetailModel userModel = new UserDetailModel();
            userModel.FirstName = user.FirstName;
            userModel.LastName = user.LastName;
            userModel.Patronymic = user.Patronymic;
            userModel.BirthDate = user.BirthDate;
            userModel.SelfDescription = user.SelfDescription;
            userModel.Id = user.Id;
            userModel.UserName = user.UserName!;
            userModel.AvatarUri = user.AvatarUri;

            return userModel;
        }

        public new async static Task<List<UserDetailModel>> FromUsers(IEnumerable<User> users)
        {
            List<UserDetailModel> usersModels = new();
            var tasks = users.Select(async user => FromUser(user));
            usersModels = (await Task.WhenAll(tasks)).ToList();

            return usersModels;
        }
    }
}
