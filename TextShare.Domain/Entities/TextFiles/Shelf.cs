using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Complaints;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Entities.TextFiles
{
    /// <summary>
    /// Класс полки.
    /// </summary>
    public class Shelf
    {
        public int ShelfId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImageUri { get; set; }
        public string? MimeType { get; set; }
        public bool CanDeleted { get; set; } = true;

        // Создатель
        public int CreatorId { get; set; }
        public User Creator { get; set; }

        // Правило доступа для полки
        public int AccessRuleId { get; set; }
        public AccessRule AccessRule { get; set; }

        // Размещенные файлы
        public ICollection<TextFile> TextFiles { get; set; } = new List<TextFile>();

        // Жалобы
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

        public override string ToString()
        {
            return $"Id: {ShelfId}. Name: {Name}. " +
                   $"Creator: {Creator}. " +
                   $"Files: {TextFiles.Count}. " +
                   $"Access Rule: {AccessRule?.AccessRuleId ?? 0}";
        }

        /// <summary>
        /// Полная информация о полке
        /// </summary>
        /// <returns></returns>
        public string GetFullInfo()
        {
            var info = new StringBuilder();

            info.AppendLine($"Id: {ShelfId}");
            info.AppendLine($"Name: {Name}");
            info.AppendLine($"Description: {Description}");
            info.AppendLine($"Created At: {CreatedAt}");
            info.AppendLine($"Image URI: {ImageUri ?? "None"}");
            info.AppendLine($"Can Be Deleted: {CanDeleted}");
            info.AppendLine($"Creator: {Creator}");
            info.AppendLine($"Number of Files: {TextFiles.Count}");

            if (AccessRule != null)
            {
                info.AppendLine("Access Rule:");
                info.AppendLine($"  Id: {AccessRule.AccessRuleId}");
                info.AppendLine($"  Available to All: {AccessRule.AvailableAll}");
                info.AppendLine($"  Available Users: {AccessRule.AvailableUsers.Count}");
                info.AppendLine($"  Available Groups: {AccessRule.AvailableGroups.Count}");
            }
            else
            {
                info.AppendLine("Access Rule: None");
            }

            return info.ToString();
        }
    }
}
