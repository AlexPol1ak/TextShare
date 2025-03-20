using System;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Models.EntityModels.UserModels
{
    /// <summary>
    /// Класс DTO для обновления пользователя.
    /// </summary>
    public class UserUpdateModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [StringLength(45, ErrorMessage = "Имя должно содержать не более 45 символов.")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я\s]+$",
            ErrorMessage = "Имя  может содержать только буквы (латинские или русские) и пробелы.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [StringLength(45, ErrorMessage = "Фамилия должна содержать не более 45 символов.")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я\s]+$",
            ErrorMessage = "Фамилия  может содержать только буквы (латинские или русские) и пробелы.")]
        public string LastName { get; set; }

        [StringLength(45, ErrorMessage = "Отчество должно содержать не более 45 символов.")]
        [RegularExpression(@"^[a-zA-Zа-яА-Я\s]+$",
                    ErrorMessage = "Отчество  может содержать только буквы (латинские или русские) и пробелы.")]

        public string? AvatarUri {  get; set; }
        public string? Patronymic { get; set; }

        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }


        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов.")]
        public string? SelfDescription { get; set; }

     
        /// <summary>
        /// Создает `UserUpdate` из объекта `User`.
        /// </summary>
        /// <param name="user">Пользователь, из которого нужно создать DTO.</param>
        /// <returns></returns>
        public static UserUpdateModel FromUser(User user)
        {
            return new UserUpdateModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                SelfDescription = user.SelfDescription,
                AvatarUri = user.AvatarUri,
            };
        }
    }
}
