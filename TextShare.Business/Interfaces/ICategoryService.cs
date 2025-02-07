using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс для сервиса управления категориями.
    /// </summary>
    public interface ICategoryService : IBaseService
    {
        /// <summary>
        /// Возвращает все категории.
        /// </summary>
        /// <returns>Список категорий</returns>
        Task<List<Category>> GetAllCategoriesAsync();

        /// <summary>
        /// Возвращает категорию по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>Возвращает категорию, если найдена, иначе null</returns>
        Task<Category?> GetCategoryByIdAsync(int id);

        /// <summary>
        /// Возвращает категорию по названию.
        /// </summary>
        /// <param name="categoryName">Название категории</param>
        /// <returns>Возвращает категорию, если найдена, иначе null</returns>
        Task<Category?> GetCategoryByNameAsync(string categoryName);

        /// <summary>
        /// Поиск категорий по условию.
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        /// <returns>Список найденных категорий</returns>
        Task<List<Category>> FindCategoriesAsync(Expression<Func<Category, bool>> predicate);

        /// <summary>
        /// Создает новую категорию.
        /// </summary>
        /// <param name="category">Создаваемая категория</param>
        /// <returns>Созданная категория</returns>
        Task<Category> CreateCategoryAsync(Category category);

        /// <summary>
        /// Удаляет категорию по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор категории</param>
        /// <returns>true, если удаление выполнено успешно, иначе false</returns>
        Task<bool> DeleteCategoryAsync(int id);

        /// <summary>
        /// Обновляет существующую категорию.
        /// </summary>
        /// <param name="category">Категория для обновления</param>
        /// <returns>Обновленная категория</returns>
        Task<Category> UpdateCategoryAsync(Category category);

        /// <summary>
        /// Проверяет, существует ли категория.
        /// Проверяет по идентификатору или другим уникальным параметрам.
        /// </summary>
        /// <param name="category">Категория для проверки</param>
        /// <returns>true, если существует, иначе false</returns>
        Task<bool> ContainsCategoryAsync(Category category);
    }
}
