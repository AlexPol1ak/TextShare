using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Domain.DTOs.ComplaintsDto
{
    /// <summary>
    /// DTO-класс для причины жалобы.
    /// </summary>
    public class ComplaintReasonsDto
    {
        public int ComplaintReasonsId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public static ComplaintReasonsDto FromComplaintReasons(ComplaintReasons complaintReasons)
        {
            return new ComplaintReasonsDto
            {
                ComplaintReasonsId = complaintReasons.ComplaintReasonsId,
                Name = complaintReasons.Name,
                Description = complaintReasons.Description
            };
        }

        public ComplaintReasons ToComplaintReasons()
        {
            return new ComplaintReasons
            {
                ComplaintReasonsId = ComplaintReasonsId,
                Name = Name,
                Description = Description
            };
        }
    }
}
