using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Models.EntityModels.TextFileModels
{
    /// <summary>
    /// Модель детального отображения файла.
    /// </summary>
    public class TextFileDetailModel
    {
        public int? CurrentUserId { get; set; } = null;
        public int TextFileId { get; set; }
        public string OriginalFileName { get; set; }
        public string UniqueFileName { get; set; }
        public string UniqueFileNameWithoutExtension { get; set; }
        public string? Description { get; set; }
        public string Extention { get; set; }
        public long Size { get; set; }

        public User Owner { get; set; }
        public List<Category> Categories { get; set; }
        public Shelf Shelf { get; set; }

        public static TextFileDetailModel FromTextFile(TextFile textFile, int?  currentUserId = null)
        {
            TextFileDetailModel model = new();
            model.CurrentUserId = currentUserId;
            model.TextFileId = textFile.TextFileId;
            model.UniqueFileName = textFile.UniqueFileName;
            model.OriginalFileName = textFile.OriginalFileName;
            model.UniqueFileNameWithoutExtension = textFile.UniqueFileNameWithoutExtension;
            model.Description = textFile.Description;
            model.Extention = textFile.Extention;
            model.Size = textFile.Size;
            model.Owner = textFile.Owner;
            model.Shelf = textFile.Shelf;
            return model;
        }
    }
}
