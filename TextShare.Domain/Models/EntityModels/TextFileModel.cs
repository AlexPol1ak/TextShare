using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Domain.Models.EntityModels
{
    /// <summary>
    /// Модель для текстового файла.
    /// </summary>
    public class TextFileModel
    {
        /// <summary>
        /// Идентификатор текстового файла.
        /// </summary>
        public int TextFileId { get; set; }

        /// <summary>
        /// Оригинальное имя файла.
        /// </summary>
        [Required]
        public string OriginalFileName { get; set; }

        /// <summary>
        /// Уникальное имя файла.
        /// </summary>
        [Required]
        public string UniqueFileName { get; set; }

        /// <summary>
        /// Описание текстового файла.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Теги текстового файла.
        /// </summary>
        public string? Tags { get; set; }

        /// <summary>
        /// Расширение файла.
        /// </summary>
        [Required]
        public string Extention { get; set; }

        /// <summary>
        /// Тип содержимого файла.
        /// </summary>
        [Required]
        public string ContentType { get; set; }

        /// <summary>
        /// Размер файла в байтах.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// URI файла.
        /// </summary>
        [Required]
        public string Uri { get; set; }

        /// <summary>
        /// Дата и время создания файла.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Идентификатор владельца файла.
        /// </summary>
        [Required]
        public int OwnerId { get; set; }

        /// <summary>
        /// Идентификатор полки, на которой находится файл.
        /// </summary>
        [Required]
        public int ShelfId { get; set; }

        /// <summary>
        /// Идентификатор правила доступа для файла.
        /// </summary>
        [Required]
        public int AccessRuleId { get; set; }

        /// <summary>
        /// Преобразует сущность <see cref="TextFile"/> в модель <see cref="TextFileModel"/>.
        /// </summary>
        /// <param name="textFile">Сущность текстового файла.</param>
        /// <returns>Модель текстового файла.</returns>
        public static TextFileModel FromTextFile(TextFile textFile)
        {
            return new TextFileModel
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

        /// <summary>
        /// Преобразует модель <see cref="TextFileModel"/> в сущность <see cref="TextFile"/>.
        /// </summary>
        /// <returns>Сущность текстового файла.</returns>
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
