using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextShare.Domain.Entities.TextFiles
{
    /// <summary>
    /// Класс  категории.
    /// </summary>
    public class Category
    {
        public int CategoryId {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Текстовые файлы.
        public ICollection<TextFileCategory> TextFileCategories { get; set; } = new List<TextFileCategory>();

        public override string ToString()
        {
            return $"Id: {CategoryId}. Name: {Name}. Number text files: {TextFileCategories.Count.ToString()}.";
        }
    }
}
