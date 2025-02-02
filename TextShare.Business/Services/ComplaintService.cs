﻿using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Business.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly IRepository<Complaint> _repositoryComplaints;

        public ComplaintService(IUnitOfWork unitOfWork)
        {
            _repositoryComplaints = unitOfWork.ComplaintRepository;
        }

        public async Task<bool> ContainsComplaintAsync(Complaint complaint)
        {
            return await _repositoryComplaints.ContainsAsync(complaint);
        }

        public async Task<Complaint> CreateComplaintAsync(Complaint complaint)
        {
            return await _repositoryComplaints.CreateAsync(complaint);
        }

        public async Task<bool> DeleteComplaintAsync(int id)
        {
            return await _repositoryComplaints.DeleteAsync(id);
        }

        public async Task<List<Complaint>> FindComplaintsAsync(Expression<Func<Complaint, bool>> predicate)
        {
            return await _repositoryComplaints.FindAsync(predicate);
        }

        public async Task<List<Complaint>> GetAllComplaintsAsync()
        {
            return await _repositoryComplaints.GetAllAsync();
        }

        public async Task<Complaint?> GetComplaintByIdAsync(int id)
        {
            return await _repositoryComplaints.GetAsync(id);
        }

        public async Task<Complaint> UpdateComplaintAsync(Complaint complaint)
        {
            return await _repositoryComplaints.UpdateAsync(complaint);
        }
    }
}
