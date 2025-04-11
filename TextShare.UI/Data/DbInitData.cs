

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

        private ComplaintReasonsDbInit complaintReasonsDbInit;
        public bool InstallComplaintReasons
        {
            get => complaintReasonsDbInit.InstallComplaintReasons;
            set => complaintReasonsDbInit.InstallComplaintReasons = value;
        }

        public DbInitData(WebApplication webApp)
        {
            userDbInit = new(webApp);
            categoryDbInit = new(webApp);
            complaintReasonsDbInit = new(webApp);
        }

        public async Task<bool> SeedData()
        {
            List<bool> flags = new();

            if (InstallUsers)
            {
                var res = await userDbInit.SeedData();
                flags.Add(res);
            }

            if (InstallCategories)
            {
                bool res = await categoryDbInit.SeedData();
                flags.Add(res);
            }

            if (InstallComplaintReasons)
            {
                bool res = await complaintReasonsDbInit.SeedData();
                flags.Add(res);
            }

            return flags.Any();
        }
    }

}
