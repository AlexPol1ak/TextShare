using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Entities.Groups
{
    /// <summary>
    /// Представляет связь между пользователем и группой, включая статус подтверждения участия.
    /// </summary>
    public class GroupMember
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsConfirmed { get; set; } = false;
    }
}
