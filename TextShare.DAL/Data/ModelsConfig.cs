using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TextShare.DAL.Converters;
using TextShare.Domain.Entities.Users;

namespace TextShare.DAL.Data
{
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
            builder.Property(u => u.SelfDescription).HasColumnType("TEXT").IsRequired(false).
                HasMaxLength(500);
            builder.Property(u => u.BirthDate).HasConversion(new DateOnlyConverter()).HasColumnType("DATE").IsRequired();
            builder.Property(u => u.RegisteredAt).HasColumnType("DATETIME").IsRequired();
        }
    }
}
