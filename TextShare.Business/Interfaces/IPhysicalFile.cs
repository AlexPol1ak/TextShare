using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Business.Interfaces
{
    /// <summary>
    /// Интерфейс управления файлами
    /// </summary>
    public interface IPhysicalFile
    {
        Task<Uri?> Get(string fileName, string? directory = null);
        Task<Dictionary<string, string>> Save(Stream fileStream,string fileName, string? directoryName = null);
        Task<bool> Delete(string fileName, string? directory = null);
        Task<bool> FileExist(string fileName, string? directory = null);
    }
}
