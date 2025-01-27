using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TextShare.DAL.Converters;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.DAL.Data
{
    /// <summary>
    /// Класс конфигурации столбцов таблиц в базе данных.
    /// </summary>
    public static class ModelsConfig
    {
        /// <summary>
        /// Конфигурация таблицы пользователя
        /// </summary>
        /// <param name="builder"></param>
        static public void UserConfig(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.Property(u => u.AvatarUri).HasMaxLength(255);
            builder.Property(u => u.FirstName).HasMaxLength(45).IsRequired();
            builder.Property(u => u.LastName).HasMaxLength(45).IsRequired();
            builder.Property(u => u.Patronymic).HasMaxLength(45);
            builder.Property(u => u.SelfDescription).HasColumnType("TEXT").IsRequired(false)
                .HasMaxLength(500);
            builder.Property(u => u.BirthDate).HasConversion(new DateOnlyConverter())
                .HasColumnType("DATE")
                .IsRequired();
            builder.Property(u => u.RegisteredAt).HasColumnType("DATETIME").IsRequired();
        }

        /// <summary>
        /// Конфигурация таблицы дружбы.
        /// </summary>
        /// <param name="builder"></param>
        static public void FriendShipsConfig(EntityTypeBuilder<Friendship> builder)
        {
            // Самоссылочная связь многие ко многим.
            builder.HasKey(f => f.Id);

            builder.HasOne(f => f.User)
                .WithMany(u => u.Friendships)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Friend)
                .WithMany(u => u.FriendRequests)
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Конфигурация таблицы группы.
        /// </summary>
        /// <param name="builder"></param>
        static public void GroupConfig(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Groups");

            builder.HasKey(g => g.GroupId);

            builder.Property(g => g.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(g => g.Description)
                .HasColumnType("TEXT")
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(g => g.CreatedAt)
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(g => g.ImageUri)
                .HasMaxLength(255);

            // Связь с создателем группы (User)
            builder.HasOne(g => g.Creator)
                .WithMany(u => u.Groups)
                .HasForeignKey(g => g.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Конфигурация таблицы участников группы.
        /// </summary>
        /// <param name="builder"></param>
        static public void GroupMemberConfig(EntityTypeBuilder<GroupMember> builder)
        {
            builder.ToTable("GroupMembers");

            // Составной ключ для связи (GroupId, UserId)
            builder.HasKey(gm => new { gm.GroupId, gm.UserId });

            // Связь с группой
            builder.HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с пользователем
            builder.HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(gm => gm.JoinedAt)
                .HasColumnType("DATETIME")
                .IsRequired();
        }

        /// <summary>
        /// Конфигурация таблицы полок.
        /// </summary>
        /// <param name="builder"></param>
        static public void ShelfConfig(EntityTypeBuilder<Shelf> builder)
        {
            builder.ToTable("Shelves");

            builder.HasKey(s => s.ShelfId);

            builder.Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.Description)
                .HasColumnType("TEXT")
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(s => s.CreatedAt)
                .HasColumnType("DATETIME")
                .IsRequired();
            // Связь с пользователем
            builder.HasOne(s => s.Creator)
                .WithMany(u => u.Shelves)  // Один пользователь может иметь много полок
                .HasForeignKey(s => s.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);  // При удалении пользователя удаляются и его полки
        }

        /// <summary>
        /// Конфигурация таблицы текстовых файлов.
        /// </summary>
        /// <param name="builder"></param>
        static public void TextFileConfig(EntityTypeBuilder<TextFile> builder)
        {
            builder.ToTable("TextFiles");

            builder.HasKey(t => t.TextFileId);
            builder.Property(t => t.OriginalName).HasMaxLength(45).IsRequired();
            builder.Property(t=>t.UniqueName).HasMaxLength(100).IsRequired();
            builder.Property(t=>t.Description).HasColumnType("TEXT")
                .IsRequired(false)
                .HasMaxLength(500);
            builder.Property(t=>t.Extention).HasMaxLength(10).IsRequired();
            builder.Property(t => t.Uri).HasMaxLength(255).IsRequired();
            //builder.Property(t=>t.Size).HasColumnType("BIGINT").IsRequired();

            // Связь с владельцем (пользователем)
            builder.HasOne(t => t.Owner)  // У файла есть один владелец
                .WithMany(u => u.TextFiles)  // Один пользователь может иметь много файлов
                .HasForeignKey(t => t.OwnerId);  // Внешний ключ для связи

            // Связь с полкой
            builder.HasOne(t => t.Shelf)  // Файл находится на одной полке
                .WithMany(s => s.TextFiles)  // Одна полка может содержать много файлов
                .HasForeignKey(t => t.ShelfId);  // Внешний ключ для связи

            // Связь с категорией
            builder.HasMany(t => t.TextFileCategories)
                .WithOne(tf => tf.TextFile)
                .HasForeignKey(tf => tf.TextFileId);
           
        }

        /// <summary>
        /// Конфигурация таблицы категорий
        /// </summary>
        /// <param name="builder"></param>
        static public void CategoryConfig(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.CategoryId);
            builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
            builder.Property(c => c.Description).HasColumnType("TEXT").IsRequired(false);

            // Связь многие ко многим с TextFile через промежуточную таблицу
            builder.HasMany(c => c.TextFileCategories)
                .WithOne(tf => tf.Category)
                .HasForeignKey(tf => tf.CategoryId);
        }

        /// <summary>
        /// Конфигурация для связи между текстовым файлом и категориями.
        /// </summary>
        /// <param name="builder"></param>
        static public void TextFileCategoryConfig(EntityTypeBuilder<TextFileCategory> builder)
        {
            builder.HasKey(tf => new { tf.TextFileId, tf.CategoryId }); // Составной ключ для связи

            builder.HasOne(tf => tf.TextFile)
                .WithMany(t => t.TextFileCategories)
                .HasForeignKey(tf => tf.TextFileId)
                .OnDelete(DeleteBehavior.Cascade); // Прописываем каскадное удаление для правильной работы

            builder.HasOne(tf => tf.Category)
                .WithMany(c => c.TextFileCategories)
                .HasForeignKey(tf => tf.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Прописываем каскадное удаление для правильной работы
        }

    }
}
