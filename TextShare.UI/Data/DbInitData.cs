

namespace TextShare.UI.Data
{
    public class DbInitData : UserDbInit
    {

        public DbInitData(WebApplication webApp) : base(webApp)
        {
        }

        public override async Task<bool> SeedData()
        {
            return await base.SeedData();
        }
    }

}
