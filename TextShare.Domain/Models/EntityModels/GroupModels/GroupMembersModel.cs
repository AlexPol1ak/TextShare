using TextShare.Domain.Models.EntityModels.UserModels;
using X.PagedList;

namespace TextShare.Domain.Models.EntityModels.GroupModels
{
    /// <summary>
    /// Модель, представляющая участников группы.
    /// </summary>
    public class GroupMembersModel
    {
        public UserModel CurrentUser { get; set; }
        public GroupDetailModel Group { get; set; }
        public IPagedList<UserModel> Members { get; set; }
    }
}
