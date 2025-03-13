using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса управления доступом к файлам  и полка.
    /// </summary>
    public interface IAccessСontrolService
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
        /// Проверяет, если ли доступ у пользователя в к файлу.
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="textFile">Файл</param>
        /// <returns>True- если пользователю разрешен доступ, false- если запрещен,
        /// null- если не удалось проверить.</returns>
        public Task<bool?> CheckTextFileAccess(User? user, TextFile textFile);

        /// <summary>
        /// Проверяет, если ли доступ у пользователя в к полке.
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="textFile">Файл</param>
        /// <returns>True- если пользователю разрешен доступ, иначе false</returns>
        public Task<bool?> CheckShelfAccess(User? user, Shelf shelf);
    }
}
