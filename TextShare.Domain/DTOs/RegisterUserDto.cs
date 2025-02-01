using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.DTOs
{
    public class RegisterUserDto
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email {  get; set; }

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

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public virtual User ToUser()
        {
            User user = new ();
            user.Email = Email;
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.Patronymic = Patronymic;
            user.BirthDate = BirthDate;
            user.SelfDescription = SelfDescription;
            user.UserName = UserName;

            return user;
        }

        public static RegisterUserDto FromUser(User user)
        {
            RegisterUserDto userDto = new RegisterUserDto();
            userDto.FirstName = user.FirstName;
            userDto.LastName = user.LastName;
            userDto.Patronymic = user.Patronymic;
            userDto.BirthDate = user.BirthDate;
            userDto.Email = user.Email!;
            userDto.SelfDescription = user.SelfDescription;
            userDto.Id = user.Id;
            userDto.UserName = user.UserName!;
            return userDto;
        }
    }
}
