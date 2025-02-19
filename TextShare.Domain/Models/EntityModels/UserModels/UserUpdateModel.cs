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
        [StringLength(45, ErrorMessage = "Имя пользователя должно содержать не более 25 символов.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$",
            ErrorMessage = "Имя пользователя может содержать только латинские буквы, цифры и символ '_'.")]
        public string UserName { get; set; }

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
        public string? Patronymic { get; set; }


        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public DateOnly BirthDate { get; set; }

        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов.")]
        public string? SelfDescription { get; set; }

        /// <summary>
        /// Обновляет существующего пользователя данными из `UserUpdateDto`.
        /// </summary>
        /// <param name="user">Объект пользователя, который нужно обновить.</param>
        public void UpdateUser(User user)
        {
            user.UserName = UserName;
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Patronymic = Patronymic;
            user.BirthDate = BirthDate;
            user.SelfDescription = SelfDescription;
        }

        /// <summary>
        /// Создает `UserUpdateDto` из объекта `User`.
        /// </summary>
        /// <param name="user">Пользователь, из которого нужно создать DTO.</param>
        /// <returns></returns>
        public static UserUpdateModel FromUser(User user)
        {
            return new UserUpdateModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                BirthDate = user.BirthDate,
                SelfDescription = user.SelfDescription
            };
        }
    }
}
