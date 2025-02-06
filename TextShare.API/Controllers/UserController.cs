using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Models;
using TextShare.Domain.Models.EntityModels.UsersDto;
using TextShare.Domain.Settings;

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
        private readonly ImageUploadSettings _imageUploadSettings;
        private readonly IPhysicalFile _physicalFile;

        public UserController(UserManager<User> userManager, IOptions<ImageUploadSettings> imageUploadSettings, IPhysicalFile physicalFile)
        {
            _userManager = userManager;
            _imageUploadSettings = imageUploadSettings.Value;
            _physicalFile = physicalFile;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        /// <param name="registerUserDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ResponseData<UserModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseData<UserModel>>> Register([FromBody] UserRegisterModel registerUserDto)
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

                ResponseData<UserModel> response = new();
                UserModel userDto = UserModel.FromUser(createdUser!);
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
        public async Task<ActionResult<UserModel>> MyProfile()
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
            UserModel userDto = UserModel.FromUser(user);
            return Ok(userDto);
        }

        [Authorize]
        [HttpPost("upload-avatar")]
        public async Task<ActionResult<ResponseData<string>>> UploadAvatar(IFormFile image)
        {
            ResponseData<string> response = new ResponseData<string>();
            // Проверка изображения 
            bool result = await validateImage(image, response);
            if (!result) return BadRequest(response);

            // Получить авторизованного пользователя
            User? user = await getAuthorizedUserDb();
            if (user == null)
            {
                response.Success = false;
                response.ErrorMessage = "Пользователь не найден.";
                return Unauthorized(response);
            }

            // Удалить старыйб аватар если есть
            if(user.AvatarUri != null)
            {             
                string fileName = Path.GetFileName(new Uri(user.AvatarUri).LocalPath);
                await _physicalFile.Delete(fileName, "Images");
                user.AvatarUri = null;
                await _userManager.UpdateAsync(user);
            }

            // Загрузить новый аватар
            Dictionary<string, string> resultDict = new();
            try
            {
                resultDict = await _physicalFile.Save(image.OpenReadStream(), image.FileName, "Images");
            }
            catch (Exception ex)
            {
                response.Data = ex.Message;
                response.Success = false;
                response.ErrorMessage = "Ошибка загрузки файла.";
                return StatusCode(StatusCodes.Status415UnsupportedMediaType, response);
            }
            // Сохранить URI
            string baseUrl = $"{Request.Scheme}://{Request.Host.Value}";
            string fileUri = $"{baseUrl}/Images/{resultDict["uniqueFileName"]}";

            user.AvatarUri = fileUri;
            await _userManager.UpdateAsync(user);

            response.Data = "Аватар успешно сохранен.";
            response.Success = true;

            return Ok(response);
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<ActionResult<UserModel>> UpdateUser([FromBody] UserUpdateModel userUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Получаем авторизованного пользователя
            User? user = await getAuthorizedUserDb();
            if (user == null)
            {
                return Unauthorized(new { message = "Пользователь не найден." });
            }

            // Обновляем данные пользователя
            userUpdateDto.UpdateUser(user);

            // Пробуем сохранить изменения
            IdentityResult result;
            try
            {
                result = await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при обновлении пользователя.", error = ex.Message });
            }

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Не удалось обновить пользователя.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return Ok(UserModel.FromUser(user));
        }

        /// <summary>
        /// Проверка изображения.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<bool> validateImage(IFormFile image, ResponseData<string> response )
        {
            await Task.CompletedTask;

            if (image == null || image.Length == 0)
            {
                response.Success = false;
                response.ErrorMessage = "Файл не загружен."; 
                return false;
            }

            if (image.Length > _imageUploadSettings.MaxFileSize)
            {
                response.Success = false;
                response.ErrorMessage = $"Файл слишком большой. Максимальный размер: {_imageUploadSettings.MaxFileSize / 1024 / 1024}MB.";
                return false;
            }

            if (!_imageUploadSettings.AllowedMimeTypes.Contains(image.ContentType))
            {
                response.Success = false;
                response.ErrorMessage = "Можно загружать только изображения (JPEG, PNG, WebP).";
                return false;
            }

            var fileExtension = Path.GetExtension(image.FileName).ToLower();
            if (!_imageUploadSettings.AllowedExtensions.Contains(fileExtension))
            {
                response.Success = false;
                response.ErrorMessage = "Файл должен быть изображением (JPEG, PNG, GIF, WebP).";
                return false ;
            }

            return true;
        }

        /// <summary>
        /// Получить авторизованного пользователя
        /// </summary>
        /// <returns></returns>
        private async Task<User?> getAuthorizedUserDb()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return null;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return user;
        }


    }
}
