

namespace TextShare.UI.Data
{
    public class DbInitData 
    {
        private UserDbInit userDbInit;
        public bool InstallUsers
        {
            get => userDbInit.InstallUsers;
            set => userDbInit.InstallUsers = value;
        }

        private CategoryDbInit categoryDbInit;
        public bool InstallCategories
        {
            get => categoryDbInit.InstallCategories;
            set => categoryDbInit.InstallCategories = value;
        }

        public DbInitData(WebApplication webApp)
        {
            userDbInit = new(webApp);
            categoryDbInit = new(webApp);
        }

        public async Task<bool> SeedData()
        {
            List<bool> flags = new();

            if (InstallUsers)
            {
               var res =  await userDbInit.SeedData();
               flags.Add(res);
            }

            if (InstallCategories)
            {
                bool res = await categoryDbInit.SeedData();
                flags.Add(res);
            }

            return flags.Any();
        }
    }

}
