using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Models.EntityModels.GroupModels;
using TextShare.Domain.Models.EntityModels.UserModels;


namespace TextShare.Domain.Models.EntityModels.AccessRuleModels
{
    /// <summary>
    /// Модель для правила доступа.
    /// </summary>
    public class AccessRuleModel
    {
        public int AccessRuleId { get; set; }
        public bool AvailableAll { get; set; }   
        public int? TextFileId { get; set; }
        public int? ShelfId { get; set; }

        public List<UserModel> AvailableUsers { get; set; } = new();
        public List<GroupModel> AvailableGroups { get; set; } = new();

        /// <summary>
        /// Создает объект <see cref="AccessRuleModel"/> на основе <see cref="AccessRule"/>.
        /// </summary>
        /// <param name="accessRule">Исходное правило доступа.</param>
        /// <returns>Объект AccessRuleModel.</returns>
        public static AccessRuleModel FromAccessRule(AccessRule accessRule)
        {
            return new AccessRuleModel
            {
                AccessRuleId = accessRule.AccessRuleId,
                AvailableAll = accessRule.AvailableAll,
                TextFileId = accessRule.TextFileId,
                ShelfId = accessRule.ShelfId,
                AvailableUsers = accessRule.AvailableUsers.Select(UserModel.FromUser).ToList(),
                AvailableGroups = accessRule.AvailableGroups.Select(GroupModel.FromGroup).ToList()
            };
        }

        /// <summary>
        /// Преобразует текущий объект <see cref="AccessRuleModel"/> в объект <see cref="AccessRule"/>.
        /// </summary>
        /// <returns>Объект AccessRule.</returns>
        public AccessRule ToAccessRule()
        {
            return new AccessRule
            {
                AccessRuleId = AccessRuleId,
                AvailableAll = AvailableAll,
                TextFileId = TextFileId
            };
        }
    }
}
