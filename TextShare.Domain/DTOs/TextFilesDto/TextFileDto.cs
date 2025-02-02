using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Domain.DTOs.TextFilesDto
{
    /// <summary>
    /// DTO-класс для текстового файла.
    /// </summary>
    public class TextFileDto
    {
        public int TextFileId { get; set; }

        [Required]
        public string OriginalFileName { get; set; }

        [Required]
        public string UniqueFileName { get; set; }

        public string? Description { get; set; }
        public string? Tags { get; set; }

        [Required]
        public string Extention { get; set; }

        [Required]
        public string ContentType { get; set; }

        public long Size { get; set; }

        [Required]
        public string Uri { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int OwnerId { get; set; }

        [Required]
        public int ShelfId { get; set; }

        [Required]
        public int AccessRuleId { get; set; }

        public static TextFileDto FromTextFile(TextFile textFile)
        {
            return new TextFileDto
            {
                TextFileId = textFile.TextFileId,
                OriginalFileName = textFile.OriginalFileName,
                UniqueFileName = textFile.UniqueFileName,
                Description = textFile.Description,
                Tags = textFile.Tags,
                Extention = textFile.Extention,
                ContentType = textFile.ContentType,
                Size = textFile.Size,
                Uri = textFile.Uri,
                CreatedAt = textFile.CreatedAt,
                OwnerId = textFile.OwnerId,
                ShelfId = textFile.ShelfId,
                AccessRuleId = textFile.AccessRuleId
            };
        }

        public TextFile ToTextFile()
        {
            return new TextFile
            {
                TextFileId = TextFileId,
                OriginalFileName = OriginalFileName,
                UniqueFileName = UniqueFileName,
                Description = Description,
                Tags = Tags,
                Extention = Extention,
                ContentType = ContentType,
                Size = Size,
                Uri = Uri,
                CreatedAt = CreatedAt,
                OwnerId = OwnerId,
                ShelfId = ShelfId,
                AccessRuleId = AccessRuleId
            };
        }
    }
}
