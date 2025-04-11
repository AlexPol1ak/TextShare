using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Models.EntityModels.AccessRuleModels
{
    /// <summary>
    /// Модель изменения правила доступа для полки
    /// </summary>
    public class ShelfAccessRuleEditModel
    {

        public int ShelfId { get; set; }
        public string ShelfName { get; set; } = string.Empty;

        public int AccessRuleId { get; set; }

        /// <summary>
        /// Список друзей пользователя
        /// </summary>
        public List<User> CreatorUserFriends { get; set; } = new();
        /// <summary>
        /// Список групп в которых состоит пользователь или является создателем.
        /// </summary>
        public List<Group> CreatorUserGroups { get; set; } = new();

        /// <summary>
        /// Список ID для передачи в POST запросе
        /// </summary>
        public List<int> AvailableUserIds { get; set; } = new();
        /// <summary>
        /// Список User для отображения в представлении
        /// </summary>
        public List<User> AvailableUsers { get; set; } = new();
        /// <summary>
        /// Список ID для передачи в POST запросе
        /// </summary>
        public List<int> AvailableGroupIds { get; set; } = new();
        /// <summary>
        /// Список Group для отображения в представлении
        /// </summary>
        public List<Group> AvailableGroups { get; set; } = new();

        public bool AvailableAll { get; set; }

        public bool ApplyToFiles { get; set; } = false;

    }
}
