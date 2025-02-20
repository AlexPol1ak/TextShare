

using Microsoft.AspNetCore.Identity;
using TextShare.Business.Interfaces;
using TextShare.Business.Services;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.UI.Data
{
    public class UserDbInit : DbInitDataAbstract
    {
        public bool InstallUsers { get; set; } = true;

        protected UserManager<User> userManager;
        protected IShelfService shelfService;
        protected IAccessRuleService accessRuleService;
        protected IUserService userService;
        public UserDbInit(WebApplication webApp) : base(webApp)
        {
            userManager = this.scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            shelfService = this.scope.ServiceProvider.GetRequiredService<IShelfService>();
            accessRuleService = this.scope.ServiceProvider.GetRequiredService<IAccessRuleService>();
            userService = this.scope.ServiceProvider.GetRequiredService<IUserService>();

        }

        public override  async Task<bool> SeedData()
        {
            if (!InstallUsers) return false;

            int userNumber = await installUser();
            return userNumber > 0;

        }
        
        private async Task<int> installUser()
        {
            int userCount = 0;
            for(int i = 1; i<11; i++)
            {
                User newUser = new();
                newUser.UserName = $"User{i}";
                newUser.FirstName = $"User{i}_FirstName";
                newUser.LastName = $"User{i}_LastName";
                newUser.BirthDate = new(2000, 01, i);
                newUser.Email = $"user{i}@mail.ru";
                newUser.Patronymic = $"User{i}_Patronymic";
                newUser.EmailConfirmed = true;

                var result = await userService.FindUsersAsync(u => u.Email == newUser.Email
                || u.UserName == newUser.UserName);

                if (result.Count > 0) continue;

                await userManager.CreateAsync(newUser, "123456");
                await createBaseShelf(newUser);
                userCount++;            
            }

            return userCount;
        }

        private async Task<Shelf> createBaseShelf(User user)
        {
            AccessRule shelfAccessRule = new();
            await accessRuleService.CreateAccessRuleAsync(shelfAccessRule);
            await accessRuleService.SaveAsync();

            Shelf baseShelf = new()
            {
                Creator = user,
                CreatorId = user.Id,
                Name = $"{user.UserName}_Полка",
                Description = "Моя первая полка",
                CanDeleted = false,
                AccessRule = shelfAccessRule,
                AccessRuleId = shelfAccessRule.AccessRuleId
            };
            await shelfService.CreateShelfAsync(baseShelf);
            await shelfService.SaveAsync();
            return baseShelf;
        }


    }
}
