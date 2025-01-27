using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Entities.TextFiles
{
    /// <summary>
    /// Класс текстового файла.
    /// </summary>
    public class TextFile
    {
        public int TextFileId { get; set; }
        public string OriginalName { get; set; }
        public string UniqueName { get; set; }
        public string? Description { get; set; }
        public string Extention { get; set; }
        public long Size {  get; set; }
        public string Uri { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Владелец
        public int OwnerId { get; set; }
        public User Owner { get; set; }

        // Полка
        public int ShelfId { get; set; }
        public Shelf Shelf { get; set; }

        // Категории
        public ICollection<TextFileCategory> TextFileCategories { get; set; } = new List<TextFileCategory>();
    }
}
