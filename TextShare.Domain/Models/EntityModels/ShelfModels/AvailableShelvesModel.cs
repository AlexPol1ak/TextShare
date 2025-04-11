using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Domain.Models.EntityModels.ShelfModels
{
    /// <summary>
    /// Модель для передачи пользователя и доступных ему полок.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="C"></typeparam>
    public class AvailableShelvesModel<U, C>
        where U : class
        where C : IEnumerable<Shelf>
    {
        public U? User { get; set; }
        public C AvailableShelves { get; set; } = default;

    }
}
