using System;
using System.Collections.Generic;

namespace TextShare.Domain.Settings
{
    /// <summary>
    /// Класс для настроек загрузки файлов.
    /// </summary>
    public class FileUploadSettings
    {
        /// <summary>
        /// Разрешённые MIME-типы файлов.
        /// </summary>
        public List<string> AllowedMimeTypes { get; set; } = new();

        /// <summary>
        /// Разрешённые расширения файлов.
        /// </summary>
        public List<string> AllowedExtensions { get; set; } = new();

        /// <summary>
        /// Максимальный размер файла в байтах.
        /// </summary>
        public long MaxFileSize { get; set; }

        /// <summary>
        /// Максимальное количество файлов на одной полке.
        /// </summary>
        public int MaxFilesPerShelf { get; set; }

        /// <summary>
        /// Количество отображаемых файлов на одной странице.
        /// </summary>
        public int FilesPerPage { get; set; }
    }
}
