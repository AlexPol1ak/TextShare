using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TextShare.DAL.Converters;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Complaints;
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
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Email).IsRequired();
            builder.HasIndex(u => u.UserName).IsUnique();
            builder.Property(u => u.UserName).IsRequired();
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

            // Жёсткая связь с создателем (обязательный пользователь)
            builder.HasOne(s => s.Creator)
                .WithMany(u => u.Shelves)
                .HasForeignKey(s => s.CreatorId)
                .IsRequired()  // Полка не может существовать без создателя
                .OnDelete(DeleteBehavior.Cascade);  // Если пользователь удаляется, удаляются и его полки

            // Жёсткая связь с AccessRule (обязательное правило доступа)
            builder.HasOne(s => s.AccessRule)
                .WithOne()
                .HasForeignKey<Shelf>(s => s.AccessRuleId)
                .IsRequired()  // Полка не может существовать без правила доступа
                .OnDelete(DeleteBehavior.Cascade);


        }

        /// <summary>
        /// Конфигурация таблицы текстовых файлов.
        /// </summary>
        /// <param name="builder"></param>
        static public void TextFileConfig(EntityTypeBuilder<TextFile> builder)
        {
            builder.ToTable("TextFiles");

            builder.HasKey(t => t.TextFileId);
            builder.Property(t => t.OriginalFileName).HasMaxLength(255).IsRequired();
            builder.Property(t => t.UniqueFileName).HasMaxLength(255).IsRequired();
            builder.HasIndex(t => t.UniqueFileName).IsUnique();
            builder.Property(t => t.Description).HasColumnType("TEXT")
                .IsRequired(false)
                .HasMaxLength(500);
            builder.Property(t => t.Extention).HasMaxLength(10).IsRequired();
            builder.Property(t => t.Uri).HasMaxLength(255).IsRequired();
            builder.Property(t => t.Tags).HasMaxLength(255).IsRequired(false);
            builder.Property(t => t.ContentType).HasMaxLength(100).IsRequired();

            // Связь с владельцем (Пользователь обязателен)
            builder.HasOne(t => t.Owner)
                .WithMany(u => u.TextFiles)
                .HasForeignKey(t => t.OwnerId)
                .IsRequired()  // Владелец обязателен
                .OnDelete(DeleteBehavior.Cascade);  // При удалении владельца удаляются и файлы

            // Связь с полкой (Полка обязательна)
            builder.HasOne(t => t.Shelf)
                .WithMany(s => s.TextFiles)
                .HasForeignKey(t => t.ShelfId)
                .IsRequired()  // Полка обязательна
                .OnDelete(DeleteBehavior.Cascade);  // При удалении полки удаляются и файлы

            // Связь с категориями
            builder.HasMany(t => t.TextFileCategories)
                .WithOne(tf => tf.TextFile)
                .HasForeignKey(tf => tf.TextFileId);

            // Связь с правилом доступа (Файл обязан иметь правило доступа)
            builder.HasOne(t => t.AccessRule)
                .WithOne(ar => ar.TextFile)
                .HasForeignKey<AccessRule>(ar => ar.TextFileId)
                .IsRequired()  // Правило доступа обязательно
                .OnDelete(DeleteBehavior.Cascade);  // При удалении файла удаляется и его правило доступа

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
            builder.HasIndex(c => c.Name).IsUnique();
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

        /// <summary>
        /// Конфигурация таблицы Жалоб
        /// </summary>
        /// <param name="builder"></param>
        static public void ComplainsConfig(EntityTypeBuilder<Complaint> builder)
        {
            builder.HasKey(c => c.ComplaintId);

            builder.HasOne(c => c.TextFile)
                .WithMany(t => t.Complaints)
                .HasForeignKey(c => c.TextFileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с ComplaintReasons (обязательная)
            builder.HasOne(c => c.ComplaintReasons)
                .WithMany(cr => cr.Complaints)
                .HasForeignKey(c => c.ComplaintReasonsId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с Author (обязательная)
            builder.HasOne(c => c.Author)
                .WithMany(u => u.MyComplaints)
                .HasForeignKey(c => c.AuthorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }

        /// <summary>
        /// Конфигурация таблицы причин жалоб
        /// </summary>
        /// <param name="builder"></param>
        static public void ComplainsReasonsConfig(EntityTypeBuilder<ComplaintReasons> builder)
        {
            builder.HasKey(c => c.ComplaintReasonsId);
            builder.Property(c => c.Name).HasMaxLength(45).IsRequired();
            builder.HasIndex(c => c.Name).IsUnique();
            builder.Property(c => c.Description).HasColumnType("TEXT")
                .HasMaxLength(300)
                .IsRequired();
        }

        /// <summary>
        /// Конфигурация таблицы правил доступа.
        /// </summary>
        /// <param name="builder"></param>
        static public void AccessRulesConfig(EntityTypeBuilder<AccessRule> builder)
        {
            builder.HasKey(a => a.AccessRuleId);

            // Связь 1 к 1 с файлом (опциональная)
            builder.HasOne(ar => ar.TextFile)
                   .WithOne(tf => tf.AccessRule)
                   .HasForeignKey<AccessRule>(ar => ar.TextFileId)
                   .IsRequired(false)  //  не обязательная
                   .OnDelete(DeleteBehavior.Cascade);

            // Связь 1 к 1 с полкой (опциональная)
            builder.HasOne(ar => ar.Shelf)
                   .WithOne(s => s.AccessRule)
                   .HasForeignKey<AccessRule>(ar => ar.ShelfId)
                   .IsRequired(false)  // не обязательная
                   .OnDelete(DeleteBehavior.Cascade);

            // Связь многие ко многим с пользователями
            builder.HasMany(ar => ar.AvailableUsers)
                   .WithMany(u => u.AccessRules)
                   .UsingEntity(j => j.ToTable("AccessRuleUsers"));

            // Связь многие ко многим с группами
            builder.HasMany(ar => ar.AvailableGroups)
                   .WithMany(g => g.AccessRules)
                   .UsingEntity(j => j.ToTable("AccessRuleGroups"));
        }



    }
}
