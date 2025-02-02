using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Groups;

namespace TextShare.Business.Interfaces
{
    public interface IGroupService
    {
        Task<List<Group>> GetAllGroupsAsync();
        Task<Group?> GetGroupByIdAsync(int id);
        Task<List<Group>> FindGroupsAsync(Expression<Func<Group, bool>> predicate);
        Task<Group> CreateGroupAsync(Group group);
        Task<bool> DeleteGroupAsync(int id);
        Task<Group> UpdateGroupAsync(Group group);
        Task<bool> ContainsGroupAsync(Group group);
    }
}
