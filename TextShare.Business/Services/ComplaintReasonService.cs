﻿using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Business.Services
{

    /// <summary> 
    ///  Сервис для управления  причинами жалоб
    /// </summary>
    public class ComplaintReasonService : BaseService, IComplaintReasonService
    {
        private readonly IRepository<ComplaintReasons> _repositoryComplaintReasons;

        public ComplaintReasonService(IUnitOfWork unitOfWork) :base(unitOfWork)
        {
            _repositoryComplaintReasons = unitOfWork.ComplaintReasonsRepository;
        }

        public async Task<bool> ContainsComplaintReasonAsync(ComplaintReasons complaintReason)
        {
            return await _repositoryComplaintReasons.ContainsAsync(complaintReason);
        }

        public async Task<ComplaintReasons> CreateComplaintReasonAsync(ComplaintReasons complaintReason)
        {
            return await _repositoryComplaintReasons.CreateAsync(complaintReason);
        }

        public async Task<bool> DeleteComplaintReasonAsync(int id)
        {
            return await _repositoryComplaintReasons.DeleteAsync(id);
        }

        public async Task<List<ComplaintReasons>> FindComplaintReasonsAsync(Expression<Func<ComplaintReasons, bool>> predicate)
        {
            return await _repositoryComplaintReasons.FindAsync(predicate);
        }

        public async Task<List<ComplaintReasons>> GetAllComplaintReasonsAsync()
        {
            return await _repositoryComplaintReasons.GetAllAsync();
        }

        public async Task<ComplaintReasons?> GetComplaintReasonByIdAsync(int id)
        {
            return await _repositoryComplaintReasons.GetAsync(id);
        }

        public async Task<ComplaintReasons> UpdateComplaintReasonAsync(ComplaintReasons complaintReason)
        {
            return await _repositoryComplaintReasons.UpdateAsync(complaintReason);
        }
    }
}
