using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TextShare.DAL.Converters;
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
    }
}
