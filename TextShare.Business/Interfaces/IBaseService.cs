using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Базовый интерфейс для сервисов.
    /// </summary>
    public interface IBaseService
    {
        /// <summary>
        /// Сохраняет изменения.
        /// </summary>
        /// <returns>Количество записей состояния, записанных в базу данных.</returns>
        Task<int> SaveAsync();

        /// <summary>
        /// Асинхронно загружает коллекцию связанных сущностей для переданной основной сущности.
        /// Используется в случаях, когда ленивая загрузка (Lazy Loading) отключена и необходимо явно загружать зависимости.
        /// </summary>
        /// <typeparam name="T">Тип основной сущности.</typeparam>
        /// <typeparam name="TProperty">Тип зависимой сущности (коллекции).</typeparam>
        /// <param name="entity">Экземпляр основной сущности, для которой требуется загрузить связанные данные.</param>
        /// <param name="navigationProperty">
        /// Лямбда-выражение, указывающее на коллекцию связанных сущностей (например, x => x.Comments).
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Выбрасывается, если переданная сущность (entity) или выражение навигационного свойства (navigationProperty) равно null.
        /// </exception>
        /// <remarks>
        /// Этот метод полезен, когда необходимо явно загружать коллекции зависимых данных, 
        /// вместо использования Include() в LINQ-запросах или если требуется подгрузить данные после получения основной сущности.
        /// </remarks>
        /// <example>
        /// Пример использования метода:
        /// <code>
        /// var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == 1);
        /// await LoadRelatedEntities(post, p => p.Comments);
        /// </code>
        /// После выполнения метода у объекта post будет загружена коллекция Comments.
        /// </example>
        public Task LoadRelatedEntitiesAsync<T, TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> navigationProperty)
                 where T : class
                 where TProperty : class;
    }
}
