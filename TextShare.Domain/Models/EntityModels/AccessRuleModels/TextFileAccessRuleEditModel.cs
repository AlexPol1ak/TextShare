using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Models.EntityModels.AccessRuleModels
{
    
    public class TextFileAccessRuleEditModel
    {
        public int TextFileId { get; set; }
        public string TextFileName { get; set; } = string.Empty;
        public string UniqueFileNameWithoutExtension {  get; set; } = string.Empty ;

        public int AccessRuleId { get; set; }

        public List<User> CreatorUserFriends { get; set; } = new();
        public List<Group> CreatorUserGroups { get; set; } = new();

        public List<int> AvailableUserIds { get; set; } = new(); // Список ID для передачи
        public List<User> AvailableUsers { get; set; } = new(); //  Список User для отображения

        public List<int> AvailableGroupIds { get; set; } = new();
        public List<Group> AvailableGroups { get; set; } = new();

        public bool AvailableAll { get; set; }

    }
}
