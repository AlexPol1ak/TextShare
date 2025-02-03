using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.DTOs.UsersDto
{
    public class UserRegisterDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(45, ErrorMessage = "Имя пользователя должно содержать не более 50 символов.")]
        public string UserName { get; set; }

        [Required]
        [StringLength(45, ErrorMessage = "Имя должно содержать не более 50 символов.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(45, ErrorMessage = "Фамилия должна содержать не более 50 символов.")]
        public string LastName { get; set; }

        [StringLength(45, ErrorMessage = "Отчество должно содержать не более 50 символов.")]
        public string? Patronymic { get; set; }

        [Required]
        public DateOnly BirthDate { get; set; }

        [StringLength(500, ErrorMessage = "Описание не должно превышать 500 символов.")]
        public string? SelfDescription { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов.")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Создает объект User из RegisterUerDto 
        /// </summary>
        /// <returns></returns>
        public User ToUser()
        {
            User user = new();
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.UserName = UserName;
            user.Patronymic = Patronymic;
            user.BirthDate = BirthDate;
            user.Email = Email;
            user.SelfDescription = SelfDescription;
            return user;
        }

        /// <summary>
        /// Создает RegisterUserDto из объекта User.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserRegisterDto FromUser(User user)
        {
            UserRegisterDto registerUserDto = new UserRegisterDto();
            registerUserDto.FirstName = user.FirstName;
            registerUserDto.LastName = user.LastName;
            registerUserDto.UserName = user.UserName!;
            registerUserDto.Patronymic = user.Patronymic;
            registerUserDto.BirthDate = user.BirthDate;
            registerUserDto.Email = user.Email!;
            registerUserDto.SelfDescription = user.SelfDescription;
            registerUserDto.Id = user.Id;
            
            return registerUserDto;
        }
    }
}
