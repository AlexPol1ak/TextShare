using System.Linq.Expressions;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса управления доступом к файлам  и полка.
    /// </summary>
    public interface IAccessControlService
    {
        /// <summary>
        /// Копирует и возвращает копию правила доступа
        /// </summary>
        /// <param name="accessRule">Правило доступа</param>
        /// <returns>Копия правила доступа не сохраненная в базе данных</returns>
        public Task<AccessRule> GetCopyAccessRule(AccessRule accessRule);

        /// <summary>
        /// Получает список файлов, доступных пользователю через группы, в которых он состоит.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="includes">Массив выражений для включения связанных данных.</param>
        /// <returns>Список файлов, доступных пользователю через группы.</returns>
        public Task<List<TextFile>> AvailableFilesFromGroups(int userId,
            params Expression<Func<TextFile, object>>[] includes);

        /// <summary>
        /// Получает список файлов, которыми поделились с пользователем напрямую.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="includes">Массив выражений для включения связанных данных.</param>
        /// <returns>Список файлов, доступных пользователю напрямую.</returns>
        public Task<List<TextFile>> AvailableFilesFromUsers(int userId,
            params Expression<Func<TextFile, object>>[] includes);

        /// <summary>
        /// Получает список всех файлов, доступных пользователю.
        /// Включает файлы, доступные через группы и напрямую.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="includes">Массив выражений для включения связанных данных.</param>
        /// <returns>Список всех доступных пользователю файлов.</returns>
        public Task<List<TextFile>> AvailableFiles(int userId,
            params Expression<Func<TextFile, object>>[] includes);

        /// <summary>
        /// Получает список полок, к которым у пользователя есть прямой доступ.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="includes">Массив выражений для включения связанных данных.</param>
        /// <returns>Список полок, доступных пользователю напрямую.</returns>
        public Task<List<Shelf>> AvailableShelvesFromUsers(int userId,
            params Expression<Func<Shelf, object>>[] includes);

        /// <summary>
        /// Получает список полок, доступных пользователю через группы, в которых он состоит.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="includes">Массив выражений для включения связанных данных.</param>
        /// <returns>Список полок, доступных пользователю через группы.</returns>
        public Task<List<Shelf>> AvailableShelvesFromGroups(int userId,
            params Expression<Func<Shelf, object>>[] includes);

        /// <summary>
        /// Получает список всех полок, доступных пользователю.
        /// Включает полки, доступные через группы и напрямую.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="includes">Массив выражений для включения связанных данных.</param>
        /// <returns>Список всех доступных пользователю полок.</returns>
        public Task<List<Shelf>> AvailableShelves(int userId,
            params Expression<Func<Shelf, object>>[] includes);

        /// <summary>
        /// Проверяет, имеет ли пользователь доступ к указанному файлу.
        /// </summary>
        /// <param name="user">Пользователь, для которого проверяется доступ. Может быть null, если пользователь не авторизован.</param>
        /// <param name="textFile">Файл, доступ к которому проверяется.</param>
        /// <returns>
        /// true – если пользователь имеет доступ к файлу,  
        /// false – если пользователь не имеет доступа,  
        /// null – если проверка не удалась (например, файл не найден).
        /// </returns>
        public Task<bool?> CheckTextFileAccess(User? user, TextFile textFile);

        /// <summary>
        /// Проверяет, имеет ли пользователь доступ к указанной полке.
        /// </summary>
        /// <param name="user">Пользователь, для которого проверяется доступ. Может быть null, если пользователь не авторизован.</param>
        /// <param name="shelf">Полка, доступ к которой проверяется.</param>
        /// <returns>
        /// true – если пользователь имеет доступ к полке,  
        /// false – если пользователь не имеет доступа,  
        /// null – если проверка не удалась (например, полка не найдена).
        /// </returns>
        public Task<bool?> CheckShelfAccess(User? user, Shelf shelf);

        /// <summary>
        /// Возвращает список полок, доступных указанной группе.
        /// </summary>
        /// <param name="groupId">ID группы, для которой проверяются доступные полки.</param>
        /// <param name="includes">Дополнительные связанные сущности для загрузки.</param>
        /// <returns>Список доступных полок.</returns>
        public Task<List<Shelf>> AvailableShelvesForGroup(int groupId,
            params Expression<Func<Shelf, object>>[] includes);

        /// <summary>
        /// Возвращает список файлов, доступных указанной группе.
        /// </summary>
        /// <param name="groupId">ID группы, для которой проверяются доступные файлы.</param>
        /// <param name="includes">Дополнительные связанные сущности для загрузки.</param>
        /// <returns>Список доступных файлов.</returns>
        public Task<List<TextFile>> AvailableFilesForGroup(int groupId,
            params Expression<Func<TextFile, object>>[] includes);
    }
}
