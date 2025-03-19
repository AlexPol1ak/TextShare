using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Domain.Models.EntityModels.TextFileModels
{
    public class TextFileDetailShortModel
    {
        public int TextFileId { get; set; }
        public string OriginalFileName { get; set; }
        public string UniqueFileName { get; set; }
        public string UniqueFileNameWithoutExtension { get; set; }
        public long Size { get; set; }

        public static async Task<TextFileDetailShortModel> FromTextFile(TextFile textFile)
        {
            await Task.CompletedTask;
            TextFileDetailShortModel model = new();
            model.TextFileId = textFile.TextFileId;
            model.OriginalFileName = textFile.OriginalFileName;
            model.UniqueFileName = textFile.UniqueFileName;
            model.UniqueFileNameWithoutExtension = textFile.UniqueFileNameWithoutExtension;
            model.Size = textFile.Size;
            return model;
        }

        public static async Task<List<TextFileDetailShortModel>> FromTextFiles(IEnumerable<TextFile> textFiles)
        {
            List<TextFileDetailShortModel> models = new();
            var tasks = textFiles.Select(async t => await FromTextFile(t));
            models = (await Task.WhenAll(tasks)).ToList();             
            return models;
        }
    }

}
