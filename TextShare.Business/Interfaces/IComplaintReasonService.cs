using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Business.Interfaces
{
    public interface IComplaintReasonService
    {
        Task<List<ComplaintReasons>> GetAllComplaintReasonsAsync();
        Task<ComplaintReasons?> GetComplaintReasonByIdAsync(int id);
        Task<List<ComplaintReasons>> FindComplaintReasonsAsync(Expression<Func<ComplaintReasons, bool>> predicate);
        Task<ComplaintReasons> CreateComplaintReasonAsync(ComplaintReasons complaintReason);
        Task<bool> DeleteComplaintReasonAsync(int id);
        Task<ComplaintReasons> UpdateComplaintReasonAsync(ComplaintReasons complaintReason);
        Task<bool> ContainsComplaintReasonAsync(ComplaintReasons complaintReason);
    }
}
