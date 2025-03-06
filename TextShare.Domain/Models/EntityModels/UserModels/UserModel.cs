using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Models.EntityModels.UserModels
{
    /// <summary>
    /// Модель для пользователя
    /// </summary>
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public string? Patronymic { get; set; }

        [Required]
        public DateOnly BirthDate { get; set; }
        public string? SelfDescription { get; set; }
        public string? AvatarUri { get; set; }

        /// <summary>
        /// Создает объект User из модели
        /// </summary>
        /// <returns></returns>
        public virtual User ToUser()
        {
            User user = new();
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.UserName = UserName;
            user.Patronymic = Patronymic;
            user.BirthDate = BirthDate;
            user.SelfDescription = SelfDescription;
            user.AvatarUri = AvatarUri;

            return user;
        }

        /// <summary>
        /// Создает модель из объекта User.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserModel FromUser(User user)
        {
            UserModel userModel = new UserModel();
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

        public async static Task<List<UserModel>> FromUsers(IEnumerable<User> users)
        {
            List<UserModel> usersModels = new();
            var tasks = users.Select(async user => FromUser(user));
            usersModels = (await Task.WhenAll(tasks)).ToList();

            return usersModels;
        }
    }
}
