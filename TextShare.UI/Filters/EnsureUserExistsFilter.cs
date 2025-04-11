using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TextShare.Domain.Entities.Users;

/// <summary>
/// Фильтр действий, который проверяет, существует ли текущий аутентифицированный пользователь в базе данных.
/// </summary>
/// <remarks>
/// Если пользователь не найден (например, удалён вручную из базы),
/// фильтр выполняет принудительный выход из системы и перенаправляет на страницу входа.
/// Это предотвращает возникновение исключений NullReferenceException в контроллерах,
/// вызванных отсутствием пользователя, несмотря на успешную аутентификацию.
/// </remarks>
public class EnsureUserExistsFilter : IAsyncActionFilter
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public EnsureUserExistsFilter(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(context.HttpContext.User);

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }
        }

        await next();
    }
}
