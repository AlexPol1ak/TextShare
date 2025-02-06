using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Models.EntityModels.UserModels
{
    public class UserRegisterModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [StringLength(45, ErrorMessage = "Имя пользователя должно содержать не более 45 символов.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [StringLength(45, ErrorMessage = "Имя должно содержать не более 45 символов.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [StringLength(45, ErrorMessage = "Фамилия должна содержать не более 45 символов.")]
        public string LastName { get; set; }

        [StringLength(45, ErrorMessage = "Отчество должно содержать не более 45 символов.")]
        public string? Patronymic { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public DateTime BirthDate { get; set; } = new DateTime(2000, 1, 1);

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [EmailAddress(ErrorMessage = "Введите корректный email.")]
        [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать не менее 6 символов.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Создает объект User из модели регистрации.
        /// </summary>
        public User ToUser()
        {
            return new User
            {
                FirstName = FirstName,
                LastName = LastName,
                UserName = UserName,
                Patronymic = Patronymic,
                BirthDate = DateOnly.FromDateTime(BirthDate),
                Email = Email
            };
        }
    }
}
