namespace TextShare.UI.Middlewares
{
    /// <summary>
    /// Middleware, блокирующий доступ к папке /TextFiles напрямую через URL.
    /// </summary>
    public class BlockTextFilesMiddleware
    {
        private readonly RequestDelegate _next;

        public BlockTextFilesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/TextFiles"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Items["ErrorMessage"] = "Ах ты хитрюга, этот способ не работает";
                return;
            }

            await _next(context);
        }
    }
    public static class BlockTextFilesMiddlewareExtensions
    {
        /// <summary>
        /// Расширение для добавления BlockTextFilesMiddleware в конвейер обработки запросов.
        /// </summary>
        public static IApplicationBuilder UseBlockTextFiles(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BlockTextFilesMiddleware>();
        }
    }
}
