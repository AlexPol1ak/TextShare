using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TextShare.Domain.Entities.Users;

namespace TextShare.DAL.Data
{
    public class TextShareContext : IdentityDbContext<User>
    {
        public DbSet<User> Users { get; set; }

        public TextShareContext(DbContextOptions<TextShareContext> options) : base(options)
        {
        }

        /// <summary>
        /// Выполняет конфигурацию таблиц.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(ModelsConfig.UserConfig);
            base.OnModelCreating(modelBuilder);
        }
    }
}
