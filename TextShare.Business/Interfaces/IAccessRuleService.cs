using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.AccessRules;

namespace TextShare.Business.Interfaces
{
    public interface IAccessRuleService
    {
        Task<List<AccessRule>> GetAllAccessRulesAsync();
        Task<AccessRule?> GetAccessRuleByIdAsync(int id);
        Task<List<AccessRule>> FindAccessRulesAsync(Expression<Func<AccessRule, bool>> predicate);
        Task<AccessRule> CreateAccessRuleAsync(AccessRule accessRule);
        Task<bool> DeleteAccessRuleAsync(int id);
        Task<AccessRule> UpdateAccessRuleAsync(AccessRule accessRule);
        Task<bool> ContainsAccessRuleAsync(AccessRule accessRule);
    }
}
