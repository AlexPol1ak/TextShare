using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextShare.Domain.Models.EntityModels.UserModels
{
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [EmailAddress(ErrorMessage = "Введите корректный email.")]
        [StringLength(100, ErrorMessage = "Email не должен превышать 100 символов.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
