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
    /// <summary>
    /// Модель изменения правила доступа для полки
    /// </summary>
    public  class ShelfAccessRuleEditModel
    {

        public int ShelfId { get; set; }
        public string ShelfName {  get; set; } = string.Empty;

        public int AccessRuleId { get; set; }

        public List<User> CreatorUserFriends { get; set; } = new();
        public List<Group> CreatorUserGroups { get; set; } = new();

        public List<int> AvailableUserIds { get; set; } = new(); // Список ID для передачи
        public List<User> AvailableUsers { get; set; } = new(); //  Список User для отображения

        public List<int> AvailableGroupIds { get; set; } = new();
        public List<Group> AvailableGroups { get; set; } = new();

        public bool AvailableAll { get; set; }

        public bool ApplyToFiles { get; set; } = false;

    }
}
