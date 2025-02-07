using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс для сервиса управления текстовыми файлами.
    /// </summary>
    public interface ITextFileService : IBaseService
    {
        /// <summary>
        /// Возвращает список всех текстовых файлов.
        /// </summary>
        /// <returns>Список текстовых файлов</returns>
        Task<List<TextFile>> GetAllTextFilesAsync();

        /// <summary>
        /// Возвращает текстовый файл по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <returns>Объект текстового файла, если найден, иначе null</returns>
        Task<TextFile?> GetTextFileByIdAsync(int id);

        /// <summary>
        /// Поиск текстовых файлов по указанному условию.
        /// </summary>
        /// <param name="predicate">Лямбда-выражение с условием поиска</param>
        /// <returns>Список текстовых файлов, удовлетворяющих условию</returns>
        Task<List<TextFile>> FindTextFilesAsync(Expression<Func<TextFile, bool>> predicate);

        /// <summary>
        /// Создает новый текстовый файл.
        /// </summary>
        /// <param name="textFile">Объект текстового файла</param>
        /// <returns>Созданный текстовый файл</returns>
        Task<TextFile> CreateTextFileAsync(TextFile textFile);

        /// <summary>
        /// Удаляет текстовый файл по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <returns>true, если удаление выполнено успешно, иначе false</returns>
        Task<bool> DeleteTextFileAsync(int id);

        /// <summary>
        /// Обновляет данные текстового файла.
        /// </summary>
        /// <param name="textFile">Обновленный объект текстового файла</param>
        /// <returns>Обновленный объект текстового файла</returns>
        Task<TextFile> UpdateTextFileAsync(TextFile textFile);

        /// <summary>
        /// Проверяет существование текстового файла в системе.
        /// </summary>
        /// <param name="textFile">Объект текстового файла</param>
        /// <returns>true, если файл существует, иначе false</returns>
        Task<bool> ContainsTextFileAsync(TextFile textFile);
    }
}
