using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Entities.TextFiles
{
    /// <summary>
    /// Класс полки.
    /// </summary>
    public class Shelf
    {
        public int ShelfId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ImageUri { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }
    }
}
