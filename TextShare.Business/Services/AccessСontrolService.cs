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

namespace TextShare.Business.Services
{
    public class AccessСontrolService : BaseService, IAccessСontrolService
    {
        private readonly IRepository<AccessRule> _accessRuleRepository;
        public AccessСontrolService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _accessRuleRepository = unitOfWork.AccessRuleRepository;
        }
    }
}
