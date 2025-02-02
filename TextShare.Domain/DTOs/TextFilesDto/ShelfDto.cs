using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.DTOs.TextFilesDto
{
    /// <summary>
    /// DTO-класс для полки.
    /// </summary>
    public class ShelfDto
    {
        public int ShelfId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ImageUri { get; set; }

        [Required]
        public int CreatorId { get; set; }

        public static ShelfDto FromShelf(Shelf shelf)
        {
            return new ShelfDto
            {
                ShelfId = shelf.ShelfId,
                Name = shelf.Name,
                Description = shelf.Description,
                CreatedAt = shelf.CreatedAt,
                ImageUri = shelf.ImageUri,
                CreatorId = shelf.CreatorId
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
                CreatorId = CreatorId
            };
        }
    }
}
