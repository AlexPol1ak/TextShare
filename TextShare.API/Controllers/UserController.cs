using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TextShare.Domain.DTOs.UsersDto;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;

namespace TextShare.API.Controllers
{
    /// <summary>
    /// Контроллер для пользователя
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="registerUserDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ResponseData<UserDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseData<UserDto>>> Register([FromBody] UserRegisterDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (registerUserDto.Password != registerUserDto.ConfirmPassword)
            {
                return BadRequest("Пароли не совпадают.");
            }

            if(registerUserDto.BirthDate >= DateOnly.FromDateTime(DateTime.Now))
            {
                return BadRequest($"Не корректная дата рождения.");
            }

            var existingUser = await _userManager.FindByEmailAsync(registerUserDto.Email);
            if (existingUser != null)
            {
                return BadRequest($"Email {registerUserDto.Email} уже зарегистрирован.");
            }

            User user = registerUserDto.ToUser();
            user.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            if (result.Succeeded)
            {
                var createdUser = await _userManager.FindByEmailAsync(user.Email);

                ResponseData<UserDto> response = new();
                UserDto userDto = UserDto.FromUser(createdUser!);
                response.Data = userDto;
                //return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, response);
                return Created(string.Empty, response);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);

        }

        /// <summary>
        /// Получить собственный профиль пользователя
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("my-profile")]
        public async Task<ActionResult<UserDto>> MyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("Идентификатор пользователя не найден.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return Unauthorized("Пользователь не найден.");
            }
            UserDto userDto = UserDto.FromUser(user);
            return Ok(userDto);
        }
    }
}
