using System.Linq.Expressions;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.AccessRules;

namespace TextShare.Business.Services
{
    /// <summary> 
    ///  Сервис для управления правилами доступа.
    /// </summary>
    public class AccessRuleService : BaseService, IAccessRuleService
    {
        private readonly IRepository<AccessRule> _repositoryAccessRules;

        public AccessRuleService(IUnitOfWork unitOfWork) : base(unitOfWork)
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

        public async Task<List<AccessRule>> GetAllAccessRulesAsync(params Expression<Func<AccessRule, object>>[] includes)
        {
            return await _repositoryAccessRules.GetAllAsync(includes);
        }

        public async Task<AccessRule?> GetAccessRuleByIdAsync(int id, params Expression<Func<AccessRule, object>>[] includes)
        {
            return await _repositoryAccessRules.GetAsync(id, includes);
        }

        public async Task<AccessRule> UpdateAccessRuleAsync(AccessRule accessRule)
        {
            return await _repositoryAccessRules.UpdateAsync(accessRule);
        }
    }
}
