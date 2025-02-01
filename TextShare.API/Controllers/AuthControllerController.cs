using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TextShare.Domain.DTOs;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;

namespace TextShare.API.Controllers
{
    /// <summary>
    /// Контроллер для регистрации и авторизации пользователя
    /// </summary>
    public class AuthControllerController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AuthControllerController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="registerUserDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (registerUserDto.Password != registerUserDto.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            User user = registerUserDto.ToUser();
            user.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if (result.Succeeded)
            {
                var createdUser = await _userManager.FindByEmailAsync(user.Email);

                ResponseData<RegisterUserDto> response = new();
                response.Data = RegisterUserDto.FromUser(createdUser);
                return Ok(response);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);

        }
    }
}
