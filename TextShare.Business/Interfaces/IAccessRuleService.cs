using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.AccessRules;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс для сервиса управления правами доступа.
    /// </summary>
    public interface IAccessRuleService : IBaseService
    {
        /// <summary>
        /// Возвращает все правила доступа.
        /// </summary>
        /// <returns>Список правил доступа</returns>
        Task<List<AccessRule>> GetAllAccessRulesAsync(params string[] includes);

        /// <summary>
        /// Возвращает правило доступа по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор правила доступа</param>
        /// <returns>Возвращает правило доступа, если найдено, иначе null</returns>
        Task<AccessRule?> GetAccessRuleByIdAsync(int id, params string[] includes);

        /// <summary>
        /// Поиск правил доступа по условию.
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        /// <returns>Список найденных правил доступа</returns>
        Task<List<AccessRule>> FindAccessRulesAsync(Expression<Func<AccessRule, bool>> predicate);

        /// <summary>
        /// Создает новое правило доступа.
        /// </summary>
        /// <param name="accessRule">Создаваемое правило доступа</param>
        /// <returns>Созданное правило доступа</returns>
        Task<AccessRule> CreateAccessRuleAsync(AccessRule accessRule);

        /// <summary>
        /// Удаляет правило доступа по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор правила доступа</param>
        /// <returns>true, если удаление выполнено успешно, иначе false</returns>
        Task<bool> DeleteAccessRuleAsync(int id);

        /// <summary>
        /// Обновляет существующее правило доступа.
        /// </summary>
        /// <param name="accessRule">Правило доступа для обновления</param>
        /// <returns>Обновленное правило доступа</returns>
        Task<AccessRule> UpdateAccessRuleAsync(AccessRule accessRule);

        /// <summary>
        /// Проверяет, существует ли правило доступа.
        /// Проверяет по идентификатору или другим уникальным параметрам.
        /// </summary>
        /// <param name="accessRule">Правило доступа для проверки</param>
        /// <returns>true, если существует, иначе false</returns>
        Task<bool> ContainsAccessRuleAsync(AccessRule accessRule);
    }
}
