using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Domain.Models.EntityModels
{
    /// <summary>
    /// DTO-класс для причины жалобы.
    /// </summary>
    public class ComplaintReasonsModel
    {
        public int ComplaintReasonsId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public static ComplaintReasonsModel FromComplaintReasons(ComplaintReasons complaintReasons)
        {
            return new ComplaintReasonsModel
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
