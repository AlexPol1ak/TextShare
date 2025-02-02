using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Groups;

namespace TextShare.Business.Services
{
    public class GroupService : IGroupService
    {
        private readonly IRepository<Group> _repositoryGroups;

        public GroupService(IUnitOfWork unitOfWork)
        {
            _repositoryGroups = unitOfWork.GroupRepository;
        }

        public async Task<bool> ContainsGroupAsync(Group group)
        {
            return await _repositoryGroups.ContainsAsync(group);
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            return await _repositoryGroups.CreateAsync(group);
        }

        public async Task<bool> DeleteGroupAsync(int id)
        {
            return await _repositoryGroups.DeleteAsync(id);
        }

        public async Task<List<Group>> FindGroupsAsync(Expression<Func<Group, bool>> predicate)
        {
            return await _repositoryGroups.FindAsync(predicate);
        }

        public async Task<List<Group>> GetAllGroupsAsync()
        {
            return await _repositoryGroups.GetAllAsync();
        }

        public async Task<Group?> GetGroupByIdAsync(int id)
        {
            return await _repositoryGroups.GetAsync(id);
        }

        public async Task<Group> UpdateGroupAsync(Group group)
        {
            return await _repositoryGroups.UpdateAsync(group);
        }
    }
}
