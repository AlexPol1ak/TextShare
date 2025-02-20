using Microsoft.AspNetCore.Mvc;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Контроллер для управления дружбой
    /// </summary>
    [Route("friends")]
    public class FriendsController : Controller
    {
        public async Task<IActionResult> MyFriends()
        {
            return View();
        }
    }
}
