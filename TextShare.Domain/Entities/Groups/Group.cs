using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Complaints;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Entities.Groups
{
    /// <summary>
    /// Класс группы пользователей.
    /// </summary>
    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImageUri { get; set; }
        public string? MimeType { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }

        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();

        // Доступ к файлам
        public ICollection<AccessRule> AccessRules { get; set; } = new List<AccessRule>();

        // Жалобы
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

        public override string ToString()
        {
            return $"Id: {GroupId}. Name: {Name}. Creator: {Creator.ToString()}. " +
                $"Number members: {Members.Count.ToString()}";
        }
    }
}
