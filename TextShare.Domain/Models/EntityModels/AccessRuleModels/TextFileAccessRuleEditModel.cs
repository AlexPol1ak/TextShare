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
        public TextFile File { get; set; }
        public List<User> OwnerUserFriends { get; set; }
        public List<GroupModel> OwnerUserGroups { get; set; }
        public List<User> AvailableUsers { get; set; }
        public List<Group> AvailableGroups { get; set; }
        public bool AvailableAll { get; set; }
    }
}
