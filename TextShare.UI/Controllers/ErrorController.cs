using Microsoft.AspNetCore.Mvc;

namespace TextShare.UI.Controllers
{
    /// <summary>
    /// Контроллер отображения ошибок.
    /// </summary>
    [Route("Error")]
    public class ErrorController : Controller
    {
        [Route("{statusCode}")]
        public async Task<IActionResult> HandleError(int statusCode)
        {
            await Task.CompletedTask;

            ViewData["StatusCode"] = statusCode;
            ViewData["ErrorMessage"] = null;
          
            if (HttpContext.Items.TryGetValue("ErrorMessage", out var  errorMessage) && errorMessage is string e)
            {
                ViewData["ErrorMessage"] = e;
            }

            return View("ErrorView");                                 
        }
    }
}
