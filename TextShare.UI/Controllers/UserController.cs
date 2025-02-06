using Microsoft.AspNetCore.Mvc;

namespace TextShare.UI.Controllers
{
    [Route("profile")]
    public class UserController : Controller
    {
        [HttpGet("my")]
        public async Task<IActionResult> DetailAsync()
        {
            return View();
        }
    }
}
