using System.Text;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Entities.AccessRules
{
    /// <summary>
    /// Класс доступа.
    /// </summary>
    public class AccessRule
    {
        public int AccessRuleId { get; set; }
        public bool AvailableAll { get; set; } = false;

        // Правило доступа для файла (если есть)
        public int? TextFileId { get; set; }
        public TextFile? TextFile { get; set; }

        // Правило доступа для полки (если есть)
        public int? ShelfId { get; set; }
        public Shelf? Shelf { get; set; }

        // Доступно пользователям.
        public ICollection<User> AvailableUsers { get; set; } = new List<User>();
        // Доступно группам
        public ICollection<Group> AvailableGroups { get; set; } = new List<Group>();

        public override string ToString()
        {
            string target = TextFile != null
                ? $"File: {TextFile.OriginalFileName}"
                : Shelf != null
                    ? $"Shelf: {Shelf.Name}"
                    : "Unknown";

            return $"Id: {AccessRuleId}. {target}. Available all: {AvailableAll}. " +
                   $"Number users: {AvailableUsers.Count}. Number groups: {AvailableGroups.Count}";
        }

        /// <summary>
        /// Полная информация о правиле доступа.
        /// </summary>
        /// <returns>Строка с детальной информацией.</returns>
        public string GetFullInfo()
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine($"Id: {AccessRuleId}");

            if (TextFile != null)
            {
                info.AppendLine($"Text file: {TextFile.OriginalFileName}");
            }
            else if (Shelf != null)
            {
                info.AppendLine($"Shelf: {Shelf.Name}");
            }

            info.AppendLine($"Available All: {AvailableAll}");

            info.AppendLine("Available users:");
            if (AvailableUsers.Count > 0)
            {
                int i = 1;
                foreach (var user in AvailableUsers)
                {
                    info.AppendLine($"{i}. {user}");
                    i++;
                }
            }
            else
            {
                info.AppendLine("No users have access.");
            }

            info.AppendLine("Available groups:");
            if (AvailableGroups.Count > 0)
            {
                int i = 1;
                foreach (var group in AvailableGroups)
                {
                    info.AppendLine($"{i}. {group}");
                    i++;
                }
            }
            else
            {
                info.AppendLine("No groups have access.");
            }

            return info.ToString();
        }

    }
}
