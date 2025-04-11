using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Models.EntityModels.ShelfModels
{
    /// <summary>
    /// Модель детальной информации полки.
    /// </summary>
    public class ShelfDetailModel
    {
        public int ShelfId { get; set; }

        public int? CurrentUserId { get; set; } = null;

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ImageUri { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }

        public int AccessRuleId { get; set; }
        public AccessRule AccessRule { get; set; }

        public ICollection<TextFile> TextFiles { get; set; } = new List<TextFile>();

        public static ShelfDetailModel FromShelf(Shelf shelf)
        {
            return new ShelfDetailModel
            {
                ShelfId = shelf.ShelfId,
                Name = shelf.Name,
                Description = shelf.Description,
                CreatedAt = shelf.CreatedAt,
                ImageUri = shelf.ImageUri,
                CreatorId = shelf.CreatorId,
                Creator = shelf.Creator,
                AccessRuleId = shelf.AccessRuleId,
                AccessRule = shelf.AccessRule,
                TextFiles = shelf.TextFiles,
            };
        }

        public Shelf ToShelf()
        {
            return new Shelf
            {
                ShelfId = ShelfId,
                Name = Name,
                Description = Description,
                CreatedAt = CreatedAt,
                ImageUri = ImageUri,
                CreatorId = CreatorId,
                Creator = Creator,
                AccessRuleId = AccessRuleId,
                AccessRule = AccessRule,
                TextFiles = TextFiles,
            };
        }
    }
}
