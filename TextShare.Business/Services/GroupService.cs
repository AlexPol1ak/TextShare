using System.Linq.Expressions;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Groups;

namespace TextShare.Business.Services
{

    /// <summary> 
    ///  Сервис для управления группами
    /// </summary>
    public class GroupService : BaseService, IGroupService
    {
        private readonly IRepository<Group> _repositoryGroups;

        public GroupService(IUnitOfWork unitOfWork) : base(unitOfWork)
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

        public async Task<List<Group>> FindGroupsAsync(Expression<Func<Group, bool>> predicate, 
            params Expression<Func<Group, object>>[] includes)
        {
            return await _repositoryGroups.FindAsync(predicate, includes);
        }

        public async Task<List<Group>> GetAllGroupsAsync(params Expression<Func<Group, object>>[] includes)
        {
            return await _repositoryGroups.GetAllAsync(includes);
        }

        public async Task<Group?> GetGroupByIdAsync(int id, params Expression<Func<Group, object>>[] includes)
        {
            return await _repositoryGroups.GetAsync(id, includes);
        }

        public async Task<Group> UpdateGroupAsync(Group group)
        {
            return await _repositoryGroups.UpdateAsync(group);
        }
    }
}
