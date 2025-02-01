using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Complaints;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Entities.TextFiles
{
    /// <summary>
    /// Класс текстового файла.
    /// </summary>
    public class TextFile
    {
        public int TextFileId { get; set; }
        public string OriginalFileName { get; set; }
        public string UniqueFileName { get; set; }
        public string? Description { get; set; }
        public string? Tags { get; set; }
        public string Extention { get; set; }
        public string ContentType { get; set; }
        public long Size {  get; set; }
        public string Uri { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Владелец
        public int OwnerId { get; set; }
        public User Owner { get; set; }

        // Полка
        public int ShelfId { get; set; }
        public Shelf Shelf { get; set; }

        // Правило доступа
        public int AccessRuleId { get; set; }
        public AccessRule AccessRule { get; set; }

        // Категории
        public ICollection<TextFileCategory> TextFileCategories { get; set; } = new List<TextFileCategory>();

        // Жалобы
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

        public override string ToString()
        {
            string info = $"Id: {TextFileId}. Original Name: {OriginalFileName}. " +
                $"Owner: {Owner.ToString()}. Shelf: {Shelf.ToString()}";    
            return info;
        }

        public string GetFullInfo()
        {
            string info = string.Empty ;

            info += $"Id: {TextFileId}\n";
            info += $"Original name: {OriginalFileName}\n";
            info += $"Unique name: {UniqueFileName}\n";
            info += $"Ext: {Extention}. Size {Size}\n";
            info += $"Content Type: {ContentType}\n";
            info += $"URI: {Uri}";
            info += $"Owner: {Owner.ToString()}\n";
            info += $"Shelf: {Shelf.ToString()}. Number complaints: {Complaints.Count.ToString()}";

            return info;
        }
    }
}
