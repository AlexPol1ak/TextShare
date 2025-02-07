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
    /// Класс текстового файла.
    /// </summary>
    public class TextFile
    {
        public int TextFileId { get; set; }
        public string OriginalFileName { get; set; }
        public string UniqueFileName { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public string Extention { get; set; }
        public string ContentType { get; set; }
        public long Size {  get; set; }
        public string Uri { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Владелец
        public int OwnerId { get; set; }
        public User Owner { get; set; }

        // Полка
        public int ShelfId { get; set; }
        public Shelf Shelf { get; set; }

        // Правило доступа
        public int AccessRuleId { get; set; }
        public AccessRule AccessRule { get; set; }

        // Категории
        public ICollection<TextFileCategory> TextFileCategories { get; set; } = new List<TextFileCategory>();

        // Жалобы
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

        public override string ToString()
        {
            return $"Id: {TextFileId}. Original Name: {OriginalFileName}. " +
                   $"Owner: {Owner}. Shelf: {Shelf}. " +
                   $"Access Rule: {AccessRule?.AccessRuleId ?? 0}";
        }

        /// <summary>
        /// Полная информация о полке
        /// </summary>
        /// <returns></returns>
        public string GetFullInfo()
        {
            var info = new StringBuilder();

            info.AppendLine($"Id: {TextFileId}");
            info.AppendLine($"Original Name: {OriginalFileName}");
            info.AppendLine($"Unique Name: {UniqueFileName}");
            info.AppendLine($"Extension: {Extention}");
            info.AppendLine($"Size: {Size} bytes");
            info.AppendLine($"Content Type: {ContentType}");
            info.AppendLine($"URI: {Uri}");
            info.AppendLine($"Created At: {CreatedAt}");
            info.AppendLine($"Owner: {Owner}");
            info.AppendLine($"Shelf: {Shelf}");
            info.AppendLine($"Number of Complaints: {Complaints.Count}");

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
