using System.Linq.Expressions;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Business.Services
{
    /// <summary> 
    ///  Сервис для управления файлами
    /// </summary>
    public class TextFileService : BaseService, ITextFileService
    {
        private readonly IRepository<TextFile> _repositoryTextFiles;

        public TextFileService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repositoryTextFiles = unitOfWork.TextFileRepository;
        }

        public async Task<bool> ContainsTextFileAsync(TextFile textFile)
        {
            return await _repositoryTextFiles.ContainsAsync(textFile);
        }

        public async Task<TextFile> CreateTextFileAsync(TextFile textFile)
        {
            return await _repositoryTextFiles.CreateAsync(textFile);
        }

        public async Task<bool> DeleteTextFileAsync(int id)
        {
            return await _repositoryTextFiles.DeleteAsync(id);
        }

        public async Task<List<TextFile>> FindTextFilesAsync(Expression<Func<TextFile, bool>> predicate, 
            params Expression<Func<TextFile, object>>[] includes)
        {
            return await _repositoryTextFiles.FindAsync(predicate,includes);
        }

        public async Task<List<TextFile>> GetAllTextFilesAsync(params Expression<Func<TextFile, object>>[] includes)
        {
            return await _repositoryTextFiles.GetAllAsync(includes);
        }

        public async Task<TextFile?> GetTextFileByIdAsync(int id, params Expression<Func<TextFile, object>>[] includes)
        {
            return await _repositoryTextFiles.GetAsync(id, includes);
        }

        public async Task<TextFile> UpdateTextFileAsync(TextFile textFile)
        {
            return await _repositoryTextFiles.UpdateAsync(textFile);
        }
    }
}
