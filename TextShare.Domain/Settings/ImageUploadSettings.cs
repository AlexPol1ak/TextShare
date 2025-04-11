namespace TextShare.Domain.Settings
{
    /// <summary>
    /// Класс для  настроек загрузки изображений.
    /// </summary>
    public class ImageUploadSettings
    {
        public List<string> AllowedMimeTypes { get; set; } = new();
        public List<string> AllowedExtensions { get; set; } = new();
        public long MaxFileSize { get; set; }
    }
}
