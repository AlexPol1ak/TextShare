using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.Users;

namespace TextShare.DAL.Data
{
    public class TextShareContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<User> Users { get; set; } 
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }

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
            modelBuilder.Entity<Friendship>(ModelsConfig.FriendShipsConfig);
            modelBuilder.Entity<Group>(ModelsConfig.GroupConfig);
            modelBuilder.Entity<GroupMember>(ModelsConfig.GroupMemberConfig);
            base.OnModelCreating(modelBuilder);
        }
    }
}
