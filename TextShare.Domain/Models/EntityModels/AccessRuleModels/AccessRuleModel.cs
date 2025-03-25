using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Models.EntityModels.UserModels;


namespace TextShare.Domain.Models.EntityModels.AccessRuleModels
{
    /// <summary>
    /// DTO-класс для правила доступа.
    /// </summary>
    public class AccessRuleModel
    {
        public int AccessRuleId { get; set; }
        public bool AvailableAll { get; set; }

        
        public int? TextFileId { get; set; }
        public int? ShelfId { get; set; }

        public List<UserModel> AvailableUsers { get; set; } = new();
        public List<GroupModel> AvailableGroups { get; set; } = new();

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
