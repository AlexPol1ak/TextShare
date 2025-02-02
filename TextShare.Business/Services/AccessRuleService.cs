using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.AccessRules;

namespace TextShare.Business.Services
{
    public class AccessRuleService : IAccessRuleService
    {
        private readonly IRepository<AccessRule> _repositoryAccessRules;

        public AccessRuleService(IUnitOfWork unitOfWork)
        {
            _repositoryAccessRules = unitOfWork.AccessRuleRepository;
        }

        public async Task<bool> ContainsAccessRuleAsync(AccessRule accessRule)
        {
            return await _repositoryAccessRules.ContainsAsync(accessRule);
        }

        public async Task<AccessRule> CreateAccessRuleAsync(AccessRule accessRule)
        {
            return await _repositoryAccessRules.CreateAsync(accessRule);
        }

        public async Task<bool> DeleteAccessRuleAsync(int id)
        {
            return await _repositoryAccessRules.DeleteAsync(id);
        }

        public async Task<List<AccessRule>> FindAccessRulesAsync(Expression<Func<AccessRule, bool>> predicate)
        {
            return await _repositoryAccessRules.FindAsync(predicate);
        }

        public async Task<List<AccessRule>> GetAllAccessRulesAsync()
        {
            return await _repositoryAccessRules.GetAllAsync();
        }

        public async Task<AccessRule?> GetAccessRuleByIdAsync(int id)
        {
            return await _repositoryAccessRules.GetAsync(id);
        }

        public async Task<AccessRule> UpdateAccessRuleAsync(AccessRule accessRule)
        {
            return await _repositoryAccessRules.UpdateAsync(accessRule);
        }
    }
}
