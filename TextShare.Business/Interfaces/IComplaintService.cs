using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс для сервиса управления жалобами.
    /// </summary>
    public interface IComplaintService : IBaseService
    {
        /// <summary>
        /// Возвращает все жалобы.
        /// </summary>
        /// <returns>Список жалоб</returns>
        Task<List<Complaint>> GetAllComplaintsAsync();

        /// <summary>
        /// Возвращает жалобу по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор жалобы</param>
        /// <returns>Возвращает жалобу, если найдена, иначе null</returns>
        Task<Complaint?> GetComplaintByIdAsync(int id);

        /// <summary>
        /// Выполняет поиск жалоб по заданному условию.
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        /// <returns>Список найденных жалоб</returns>
        Task<List<Complaint>> FindComplaintsAsync(Expression<Func<Complaint, bool>> predicate);

        /// <summary>
        /// Создает новую жалобу.
        /// </summary>
        /// <param name="complaint">Создаваемая жалоба</param>
        /// <returns>Созданная жалоба</returns>
        Task<Complaint> CreateComplaintAsync(Complaint complaint);

        /// <summary>
        /// Удаляет жалобу по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор жалобы</param>
        /// <returns>true, если удаление выполнено успешно, иначе false</returns>
        Task<bool> DeleteComplaintAsync(int id);

        /// <summary>
        /// Обновляет существующую жалобу.
        /// </summary>
        /// <param name="complaint">Жалоба для обновления</param>
        /// <returns>Обновленная жалоба</returns>
        Task<Complaint> UpdateComplaintAsync(Complaint complaint);

        /// <summary>
        /// Проверяет, существует ли жалоба.
        /// </summary>
        /// <param name="complaint">Жалоба для проверки</param>
        /// <returns>true, если существует, иначе false</returns>
        Task<bool> ContainsComplaintAsync(Complaint complaint);
    }
}
