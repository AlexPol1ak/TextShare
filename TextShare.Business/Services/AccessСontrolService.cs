using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.DAL.Repositories;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Complaints;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Services
{
    /// <summary>
    /// Класс сервиса для управления доступом к файлам и полкам.
    /// </summary>
    public class AccessСontrolService : BaseService, IAccessСontrolService
    {
        private readonly IRepository<AccessRule> _accessRuleRepository;
        public AccessСontrolService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _accessRuleRepository = unitOfWork.AccessRuleRepository;
        }

        public async Task<AccessRule> GetCopyAccessRule(AccessRule accessRule)
        {
            await Task.CompletedTask;
            AccessRule accessRuleCopy = new()
            {
                AvailableAll = accessRule.AvailableAll,
                AvailableGroups = accessRule.AvailableGroups.ToList(),
                AvailableUsers = accessRule.AvailableUsers.ToList()
            };
            return accessRuleCopy;
        }
    }
}
