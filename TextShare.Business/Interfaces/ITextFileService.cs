using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Business.Interfaces
{
    public interface ITextFileService
    {
        Task<List<TextFile>> GetAllTextFilesAsync();
        Task<TextFile?> GetTextFileByIdAsync(int id);
        Task<List<TextFile>> FindTextFilesAsync(Expression<Func<TextFile, bool>> predicate);
        Task<TextFile> CreateTextFileAsync(TextFile textFile);
        Task<bool> DeleteTextFileAsync(int id);
        Task<TextFile> UpdateTextFileAsync(TextFile textFile);
        Task<bool> ContainsTextFileAsync(TextFile textFile);
    }
}
