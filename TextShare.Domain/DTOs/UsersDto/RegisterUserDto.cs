using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.DTOs.UsersDto
{
    public class RegisterUserDto : UserDto
    {

        [Required]
        [DataType(DataType.EmailAddress)]
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
        public override User ToUser()
        {
            User user = base.ToUser();
            user.Email = Email;
            return user;
        }

        /// <summary>
        /// Создает RegisterUserDto из объекта User.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public new static RegisterUserDto FromUser(User user)
        {
            RegisterUserDto registerUserDto = new RegisterUserDto();
            registerUserDto.FirstName = user.FirstName;
            registerUserDto.LastName = user.LastName;
            registerUserDto.Patronymic = user.Patronymic;
            registerUserDto.BirthDate = user.BirthDate;
            registerUserDto.Email = user.Email!;
            registerUserDto.SelfDescription = user.SelfDescription;
            registerUserDto.Id = user.Id;
            registerUserDto.UserName = user.UserName!;
            return registerUserDto;
        }
    }
}
