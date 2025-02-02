using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Business.Interfaces
{
    public interface IComplaintService
    {
        Task<List<Complaint>> GetAllComplaintsAsync();
        Task<Complaint?> GetComplaintByIdAsync(int id);
        Task<List<Complaint>> FindComplaintsAsync(Expression<Func<Complaint, bool>> predicate);
        Task<Complaint> CreateComplaintAsync(Complaint complaint);
        Task<bool> DeleteComplaintAsync(int id);
        Task<Complaint> UpdateComplaintAsync(Complaint complaint);
        Task<bool> ContainsComplaintAsync(Complaint complaint);
    }
}
