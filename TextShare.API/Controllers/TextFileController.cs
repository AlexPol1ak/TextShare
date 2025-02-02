using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TextShare.Business.Interfaces;
using TextShare.Domain.Utils;

namespace TextShare.API.Controllers
{
    /// <summary>
    /// Контроллер для загрузки файлов
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TextFileController : ControllerBase
    {
        private readonly IPhysicalFile _physicalFile;

        public TextFileController(IPhysicalFile physicalFile)
        {
            _physicalFile = physicalFile;
        }

        /// <summary>
        /// Загрузить текстовый файл.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<ActionResult> UploadTextFile(IFormFile file)
        {

            var res = await _physicalFile.Save(file.OpenReadStream(), file.FileName, "TextFiles");

            return Ok(res);
        }
    }
}
