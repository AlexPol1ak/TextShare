﻿using Microsoft.EntityFrameworkCore;
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
            var query = await _repositoryGroups.FindAsync(predicate, includes);
            return await query.ToListAsync();
        }

        public async Task<List<Group>> GetAllGroupsAsync(params Expression<Func<Group, object>>[] includes)
        {
            var query = await _repositoryGroups.GetAllAsync(includes);
            return await query.ToListAsync();
        }

        public async Task<Group?> GetGroupByIdAsync(int id, params Expression<Func<Group, object>>[] includes)
        {
            return await _repositoryGroups.GetAsync(id, includes);
        }

        public async Task<Group> UpdateGroupAsync(Group group)
        {
            return await _repositoryGroups.UpdateAsync(group);
        }

        public async Task<List<Group>> GetUserCreatedGroupsAsync(int userId,
            params Expression<Func<Group, object>>[] includes)
        {
            IQueryable<Group> groupsQuery = await _repositoryGroups.FindAsync(
                g => g.CreatorId == userId, includes
                );

            return await groupsQuery.ToListAsync();
        }

        public async Task<List<Group>> GetUserMemberGroupsAsync(int userId,
            params Expression<Func<Group, object>>[] includes)
        {
            var groups = await FindGroupsAsync(
                        g => g.Members.Any(m => m.UserId == userId && m.IsConfirmed == true),
                        includes
                    );

            return groups;
        }

        public async Task<List<Group>> GetUserOutRequestsGroups(int userId,
            params Expression<Func<Group, object>>[] includes
            )
        {
            var groups = await FindGroupsAsync(
                g => g.Members.Any(m => m.UserId == userId && m.IsConfirmed == false),
                includes);

            return groups;
        }
    }
}
