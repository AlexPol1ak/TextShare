using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TextShare.Business.Interfaces;
using TextShare.Business.Services;
using TextShare.Domain.Entities.Users;
using TextShare.Domain.Settings;

namespace TextShare.UI.Controllers
{
    public class TextFileController : BaseController
    {
        private readonly FileUploadSettings _fileUploadSettings;
        private readonly UserManager<User> _userManager;
        private readonly ShelvesSettings _shelvesSettings;
        private readonly IUserService _userService;
        private readonly IShelfService _shelfService;
        public TextFileController(
            IPhysicalFile physicalFile,
            IOptions<ImageUploadSettings> imageUploadSettingsOptions,
            IOptions<FileUploadSettings> fileUploadSettings,
            IOptions<ShelvesSettings> shelvesSettingsOptions,
            IShelfService shelfService,
            UserManager<User> userManager,
            IUserService userService
            ) 
            : base(physicalFile, imageUploadSettingsOptions)
        {
            _fileUploadSettings = fileUploadSettings.Value;
            _shelfService = shelfService;
            _userManager = userManager;
            _userService = userService;
            _shelvesSettings = shelvesSettingsOptions.Value;
        }

        public async Task<IActionResult> Upload()
        {
            return View();
        }

        public async Task<IActionResult> Download()
        {
            return View();
        }
    }
}
