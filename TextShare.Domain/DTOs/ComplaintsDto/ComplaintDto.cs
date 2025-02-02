using System;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Domain.DTOs.ComplaintsDto
{
    /// <summary>
    /// DTO-класс для жалобы на файл.
    /// </summary>
    public class ComplaintDto
    {
        public int ComplaintId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool Confirmed { get; set; } = false;

        [Required]
        public int TextFileId { get; set; }

        [Required]
        public int ComplaintReasonsId { get; set; }

        [Required]
        public int AuthorId { get; set; }

        public static ComplaintDto FromComplaint(Complaint complaint)
        {
            return new ComplaintDto
            {
                ComplaintId = complaint.ComplaintId,
                CreatedAt = complaint.CreatedAt,
                Confirmed = complaint.Confirmed,
                TextFileId = complaint.TextFileId,
                ComplaintReasonsId = complaint.ComplaintReasonsId,
                AuthorId = complaint.AuthorId
            };
        }

        public Complaint ToComplaint()
        {
            return new Complaint
            {
                ComplaintId = ComplaintId,
                CreatedAt = CreatedAt,
                Confirmed = Confirmed,
                TextFileId = TextFileId,
                ComplaintReasonsId = ComplaintReasonsId,
                AuthorId = AuthorId
            };
        }
    }
}
