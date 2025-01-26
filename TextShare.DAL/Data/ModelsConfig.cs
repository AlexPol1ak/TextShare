using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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

        }
    }
}
