using System.Linq.Expressions;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Business.Services
{

    /// <summary> 
    ///  Сервис для управления жалобами
    /// </summary>
    public class ComplaintService : BaseService, IComplaintService
    {
        private readonly IRepository<Complaint> _repositoryComplaints;

        public ComplaintService(IUnitOfWork unitOfWork) : base(unitOfWork)
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

        public async Task<List<Complaint>> GetAllComplaintsAsync(params Expression<Func<Complaint, object>>[] includes)
        {
            return await _repositoryComplaints.GetAllAsync(includes);
        }

        public async Task<Complaint?> GetComplaintByIdAsync(int id, params Expression<Func<Complaint, object>>[] includes)
        {
            return await _repositoryComplaints.GetAsync(id, includes);
        }

        public async Task<Complaint> UpdateComplaintAsync(Complaint complaint)
        {
            return await _repositoryComplaints.UpdateAsync(complaint);
        }
    }
}
