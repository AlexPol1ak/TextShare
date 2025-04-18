﻿namespace TextShare.Domain.Entities.TextFiles
{
    /// <summary>
    /// Промежуточный класс для связи текстовых файлов и категорий.
    /// </summary>
    public class TextFileCategory
    {
        public int TextFileId { get; set; }
        public TextFile TextFile { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
