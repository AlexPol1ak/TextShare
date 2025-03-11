using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.AccessRules;

namespace TextShare.Business.Interfaces
{
    public interface IAccessСontrolService
    {
        public Task<AccessRule> GetCopyAccessRule(AccessRule accessRule);
    }
}
