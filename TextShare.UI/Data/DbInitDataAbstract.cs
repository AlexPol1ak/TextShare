namespace TextShare.UI.Data
{
    /// <summary>
    /// Базовый класс установки начальных данных.
    /// </summary>
    abstract public class DbInitDataAbstract
    {
        protected readonly WebApplication webApp;
        protected readonly IServiceScope scope;

        public DbInitDataAbstract(WebApplication webApp)
        {
            this.webApp = webApp;
            scope = webApp.Services.CreateScope();
        }

        public abstract Task<bool> SeedData();
    }
}
