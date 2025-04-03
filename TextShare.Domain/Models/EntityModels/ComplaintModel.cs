using System;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Domain.Models.EntityModels
{
    /// <summary>
    /// Модель для жалобы на файл.
    /// </summary>
    public class ComplaintModel
    {
        public int ComplaintId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool Confirmed { get; set; } = false;

        public int? TextFileId { get; set; }
        public int? ShelfId { get; set; }
        public int? GroupId { get; set; }

        [Required]
        public int ComplaintReasonsId { get; set; }

        [Required]
        public int AuthorId { get; set; }

        public static ComplaintModel FromComplaint(Complaint complaint)
        {
            var complaintModel = new ComplaintModel();
            complaintModel.ComplaintId = complaint.ComplaintId;
            complaintModel.CreatedAt = complaint.CreatedAt;
            complaintModel.Confirmed = complaint.Confirmed;
            complaintModel.ComplaintReasonsId = complaint.ComplaintReasonsId;
            complaintModel.AuthorId = complaint.AuthorId;

            if(complaint.TextFile != null) 
                complaintModel.TextFileId = complaint.TextFileId;
            else if(complaint.ShelfId != null)
                complaintModel.ShelfId = complaint.ShelfId;
            else if(complaintModel.GroupId != null)
                complaint.GroupId = complaintModel.GroupId; 

                return complaintModel;
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
