using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Models.EntityModels.GroupModels
{
    public enum UserGroupRelationStatus
    {
        [Display(Name = "Владелец")]
        Creator = 0,

        [Display(Name = "Заявка запрошена")]
        Requsted = 1,

        [Display(Name = "Участник группы")]
        Member = 2,

        [Display(Name = "Не учатсник группы")]
        NotMember = 3,

    }

    /// <summary>
    /// Модель для детального отображения группы.
    /// </summary>
    public class GroupDetailModel
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUri { get; set; }
        public User Creator { get; set; }

        public int CountRequests { get; set; } = 0;

        public UserGroupRelationStatus UserGroupRelationStatus { get; set; }

        public static async Task<GroupDetailModel> FromGroup(Group group)
        {
            await Task.CompletedTask;
            GroupDetailModel groupDetailModel = new();
            groupDetailModel.GroupId = group.GroupId;
            groupDetailModel.Name = group.Name;
            groupDetailModel.Description = group.Description;
            groupDetailModel.CreatedAt = group.CreatedAt;
            groupDetailModel.ImageUri = group.ImageUri;
            groupDetailModel.Creator = group.Creator;

            if (group.Members != null && group.Members.Count > 0)
            {
                groupDetailModel.CountRequests = group.Members.Where(m => m.IsConfirmed == false).Count();
            }

            return groupDetailModel;
        }

        public static async Task<List<GroupDetailModel>> FromGroup(IEnumerable<Group> groups)
        {
            var tasks = groups.Select(async group => await FromGroup(group));
            List<GroupDetailModel> groupsModel = (await Task.WhenAll(tasks)).ToList();
            return groupsModel;
        }

    }
}
