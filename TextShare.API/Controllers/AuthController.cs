﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TextShare.Business.Interfaces;
using TextShare.Domain.DTOs.UsersDto;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;

namespace TextShare.API.Controllers
{
    /// <summary>
    /// Контроллер для регистрации и авторизации пользователя
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenService = tokenService;
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

            var existingUser = await _userManager.FindByEmailAsync(registerUserDto.Email);
            if(existingUser != null)
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
                UserDto userDto =  UserDto.FromUser(createdUser!);
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
        /// Аутентификация пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Пользователь не найден.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Неверный email или пароль.");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });

        }

        /// <summary>
        /// Обновление Access-токена по Refresh-токену
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token отсутствует.");
            }

            // Находим пользователя по refresh token
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized("Refresh token недействителен или истек.");
            }

            // Генерация нового access token
            var newAccessToken = _tokenService.GenerateAccessToken(user);

            // Генерация нового refresh token
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Обновляем refresh token в базе данных
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        /// <summary>
        ///  Завершает текущую аутентификацию
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Получаем идентификатор пользователя из токена
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("Пользователь не найден.");
            }

            // Находим пользователя в базе данных
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized("Пользователь не найден.");
            }

            // Удаляем refresh token
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            // Сохраняем изменения в базе данных
            await _userManager.UpdateAsync(user);

            return Ok(new { Message = "Вы успешно вышли из системы." });
        }

        /// <summary>
        /// Проверка аутентификации.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("protected-endpoint")]
        public async Task<IActionResult> ProtectedEndpoint()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(new { Message = $"User Id {userId}" });
        }

        /// <summary>
        /// Проверяет, является ли токен действительным
        /// </summary>
        /// <param name="token">Токен для проверки</param>
        /// <returns>Статус валидации токена</returns>
        [HttpPost("validate-token")]
        public IActionResult ValidateToken([FromBody] string token)
        {
            // Проверяем, валиден ли токен
            var isValid = _tokenService.VerifyAccessToken(token);

            if (isValid)
            {
                return Ok(new { Message = "Token is valid." });
            }
            else
            {
                return Unauthorized(new { Message = "Invalid or expired token." });
            }
        }
    }
}
