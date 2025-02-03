using System;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.DTOs.UsersDto
{
    /// <summary>
    /// Класс DTO для обновления пользователя.
    /// </summary>
    public class UserUpdateDto
    {

        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Имя пользователя должно содержать не более 50 символов.")]
        public string UserName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Имя должно содержать не более 50 символов.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Фамилия должна содержать не более 50 символов.")]
        public string LastName { get; set; }

        [StringLength(50, ErrorMessage = "Отчество должно содержать не более 50 символов.")]
        public string? Patronymic { get; set; }

        [Required]
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
        public static UserUpdateDto FromUser(User user)
        {
            return new UserUpdateDto
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
