using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Entities.Complaints
{
    /// <summary>
    /// Класс жалобы на файл.
    /// </summary>
    public class Complaint
    {
        public int ComplaintId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Confirmed { get; set; } = false;

        // Файл
        public int TextFileId { get; set; }
        public TextFile TextFile { get; set; }

        // Причина
        public int ComplaintReasonsId { get; set; }
        public ComplaintReasons ComplaintReasons { get; set; }

        // Автор
        public int AuthorId { get; set; } 
        public User Author { get; set; }

        public override string ToString()
        {
            string info = string.Empty;
            info += $"Id: {ComplaintId}. Created At: {CreatedAt.ToString()}. Confirmed: {Confirmed}\n" +
                $"File: {TextFile.ToString()}. Reasons: {ComplaintReasons.ToString()}\n" +
                $"Author: {Author.ToString()}";

            return info;
        }
    }
}
