using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.DTOs.TextFilesDto;
using TextShare.Domain.DTOs.UsersDto;
using TextShare.Domain.DTOs.GroupsDto;

namespace TextShare.Domain.DTOs.AccessRulesDto
{
    /// <summary>
    /// DTO-класс для правила доступа.
    /// </summary>
    public class AccessRuleDto
    {
        public int AccessRuleId { get; set; }
        public bool AvailableAll { get; set; }

        [Required]
        public int TextFileId { get; set; }

        public List<UserDto> AvailableUsers { get; set; } = new();
        public List<GroupDto> AvailableGroups { get; set; } = new();

        public static AccessRuleDto FromAccessRule(AccessRule accessRule)
        {
            return new AccessRuleDto
            {
                AccessRuleId = accessRule.AccessRuleId,
                AvailableAll = accessRule.AvailableAll,
                TextFileId = accessRule.TextFileId,
                AvailableUsers = accessRule.AvailableUsers.Select(UserDto.FromUser).ToList(),
                AvailableGroups = accessRule.AvailableGroups.Select(GroupDto.FromGroup).ToList()
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
