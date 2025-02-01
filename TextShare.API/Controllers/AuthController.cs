using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        public AuthController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
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
        public async Task<ActionResult<string>> Login([FromBody] UserLoginDto model)
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

            // Попытка входа.
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Неверный email или пароль.");
            }

            // Генерация JWT-токена
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
      
        /// <summary>
        /// Генерация токенов.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
