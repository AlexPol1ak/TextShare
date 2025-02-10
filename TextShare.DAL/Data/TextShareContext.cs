using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Complaints;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.DAL.Data
{
    /// <summary>
    /// Класс контекста базы данных.
    /// </summary>
    public class TextShareContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<User> Users { get; set; } // Пользователи 
        public DbSet<Friendship> Friendships { get; set; } // Друзья
        public DbSet<Group> Groups { get; set; } // Группы
        public DbSet<GroupMember> GroupMembers { get; set; } //Участники групп
        public DbSet<Shelf> Shelves { get; set; } //Полки
        public DbSet<TextFile> TextFiles { get; set; } //Текстовые файлы пользователей
        public DbSet<AccessRule> AccessRules { get; set; } //  Правила доступа к файлам.
        public DbSet<Category> Categories { get; set; } // Категории
        // Промежут. таблица для связи файлов и категорий
        public DbSet<TextFileCategory> TextFileCategories { get; set; }
        public DbSet<Complaint> Complaints { get; set; } // Жалобы на файл
        public DbSet<ComplaintReasons> ComplaintReasons { get; set; }  // Причина жалоб

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
            modelBuilder.Entity<Shelf>(ModelsConfig.ShelfConfig);
            modelBuilder.Entity<TextFile>(ModelsConfig.TextFileConfig);
            modelBuilder.Entity<Category>(ModelsConfig.CategoryConfig);
            modelBuilder.Entity<TextFileCategory>(ModelsConfig.TextFileCategoryConfig);
            modelBuilder.Entity<Complaint>(ModelsConfig.ComplainsConfig);
            modelBuilder.Entity<ComplaintReasons>(ModelsConfig.ComplainsReasonsConfig);
            modelBuilder.Entity<AccessRule>(ModelsConfig.AccessRulesConfig);

            base.OnModelCreating(modelBuilder);
        }
    }
}
