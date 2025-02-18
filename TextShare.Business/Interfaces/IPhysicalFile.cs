﻿namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс управления файлами
    /// </summary>
    public interface IPhysicalFile
    {
        Task<string?> GetFullPath(string fileName, string? directory = null);
        Task<Dictionary<string, string>> Save(Stream fileStream, string fileName, string? directoryName = null);
        Task<bool> Delete(string fileName, string? directory = null);
        Task<bool> FileExist(string fileName, string? directory = null);
    }
}
