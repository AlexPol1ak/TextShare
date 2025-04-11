using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Domain.Models.EntityModels.ShelfModels
{
    /// <summary>
    /// Модель для удаления полки.
    /// </summary>
    public class ShelfDeleteModel
    {
        public int ShelfId { get; set; }
        public string Name { get; set; }

        public int CountFiles { get; set; }

        public static ShelfDeleteModel FromShelf(Shelf shelf)
        {
            ShelfDeleteModel shelfDelete = new()
            {
                ShelfId = shelf.ShelfId,
                Name = shelf.Name,
                CountFiles = shelf.TextFiles.Count
            };
            return shelfDelete;
        }
    }

}
