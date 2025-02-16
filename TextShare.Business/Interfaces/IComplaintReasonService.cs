using System.Linq.Expressions;
using TextShare.Domain.Entities.Complaints;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс для сервиса управления причинами жалоб.
    /// </summary>
    public interface IComplaintReasonService : IBaseService
    {
        /// <summary>
        /// Возвращает все причины жалоб.
        /// </summary>
        /// <returns>Список причин жалоб</returns>
        Task<List<ComplaintReasons>> GetAllComplaintReasonsAsync(params Expression<Func<ComplaintReasons, object>>[] includes);

        /// <summary>
        /// Возвращает причину жалобы по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор причины жалобы</param>
        /// <returns>Возвращает причину жалобы, если найдена, иначе null</returns>
        Task<ComplaintReasons?> GetComplaintReasonByIdAsync(int id,
            params Expression<Func<ComplaintReasons, object>>[] includes);

        /// <summary>
        /// Выполняет поиск причин жалоб по заданному условию.
        /// </summary>
        /// <param name="predicate">Условие поиска</param>
        /// <returns>Список найденных причин жалоб</returns>
        Task<List<ComplaintReasons>> FindComplaintReasonsAsync(Expression<Func<ComplaintReasons, bool>> predicate,
            params Expression<Func<ComplaintReasons, object>>[] includes);

        /// <summary>
        /// Создает новую причину жалобы.
        /// </summary>
        /// <param name="complaintReason">Создаваемая причина жалобы</param>
        /// <returns>Созданная причина жалобы</returns>
        Task<ComplaintReasons> CreateComplaintReasonAsync(ComplaintReasons complaintReason);

        /// <summary>
        /// Удаляет причину жалобы по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор причины жалобы</param>
        /// <returns>true, если удаление выполнено успешно, иначе false</returns>
        Task<bool> DeleteComplaintReasonAsync(int id);

        /// <summary>
        /// Обновляет существующую причину жалобы.
        /// </summary>
        /// <param name="complaintReason">Причина жалобы для обновления</param>
        /// <returns>Обновленная причина жалобы</returns>
        Task<ComplaintReasons> UpdateComplaintReasonAsync(ComplaintReasons complaintReason);

        /// <summary>
        /// Проверяет, существует ли причина жалобы.
        /// </summary>
        /// <param name="complaintReason">Причина жалобы для проверки</param>
        /// <returns>true, если существует, иначе false</returns>
        Task<bool> ContainsComplaintReasonAsync(ComplaintReasons complaintReason);
    }
}
