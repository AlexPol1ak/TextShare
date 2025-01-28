using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public int TextFileId { get; set; }
        public TextFile TextFile { get; set; }

        // Доступно пользователям.
        public ICollection<User> AvailableUsers { get; set; } = new List<User>();
        // Доступно группам
        public ICollection<Group> AvailableGroups { get; set; } = new List<Group>();

        public override string ToString()
        {
            return $"Id: {AccessRuleId}. File: {TextFile.ToString()}. Available all: {AvailableAll}. " +
                $"Number users: {AvailableUsers.Count.ToString()}. Number groups: {AvailableGroups.Count.ToString()}";
        }

        /// <summary>
        /// Полная информация о правиле.
        /// </summary>
        /// <returns></returns>
        public string GetFullInfo()
        {
            string info = string.Empty;

            info += $"Id: {AccessRuleId}. Text file: {TextFile.OriginalName}\n";
            info += $"Available All {AvailableAll}\n";
            info += $"Available users:\n";
            if (AvailableUsers != null && AvailableUsers.Count > 0)
            {
                int i = 1;
                foreach(var user in AvailableUsers)
                {
                    info += $"{i}.{user.ToString()}\n";
                    i++;
                }
            }
            info += $"Available groups:\n";
            if (AvailableGroups != null && AvailableGroups.Count > 0)
            {
                int i = 1;
                foreach (var group in AvailableGroups)
                {
                    info += $"{i}.{group.ToString()}\n";
                    i++;
                }
            }
            return info;
        }
    }
}
