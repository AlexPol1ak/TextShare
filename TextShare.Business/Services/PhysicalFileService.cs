using System.IO;
using TextShare.Business.Interfaces;

namespace TextShare.Business.Services
{
    /// <summary>
    /// Сервис физического управления файлом.
    /// </summary>
    public class PhysicalFileService : IPhysicalFile
    {
        private readonly string _root;

        public PhysicalFileService(string root)
        {
            this._root = root;
        }

        public async Task<Dictionary<string, string?>> GetFile(string fileName, string? directoryName = null)
        {
            Dictionary<string, string?> result = new() { 
                {"relativePath", null},
                {"fullPath", null }
            };
            if(await FileExist(fileName, directoryName))
            {
                if(directoryName != null)
                    result["relativePath"] = Path.Combine(directoryName, fileName);
                else
                    result["relativePath"] = Path.Combine(fileName);
            }
            string? fullPath = await GetFullPath(fileName, directoryName);
            if(fullPath != null)
                result["fullPath"] = fullPath;

            return result;
        }

        /// <summary>
        /// Возвращает полный путь к файлу
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        public async Task<string?> GetFullPath(string fileName, string? directoryName = null)
        {
            string? filePath = null;
            if(await FileExist(fileName, directoryName))
            {
                if (!string.IsNullOrEmpty(directoryName))
                    filePath = Path.Combine(_root, directoryName, fileName);
                else
                    filePath = Path.Combine(_root, fileName);
            }

            return filePath;
        }

        /// <summary>
        /// Сохраняет файл.
        /// </summary>
        /// <param name="fileStream">Поток файла</param>
        /// <param name="fileName">Имя файла с расширениемИмя файла с расширением</param>
        /// <param name="directoryName">Директория</param>
        /// <returns>
        ///  Объект <see cref="ResponseData{Dictionary{string, string}}"/> с данными о сохранении файла:<br/>
        /// - "originalFileName": оригинальное имя файла с расширением <br/>
        /// - "uniqueFileName": уникальное имя файла с расширением <br/>
        /// - "size": размер в байтах <br/>
        /// - "type": расширение <br/>
        /// </returns>
        public async Task<Dictionary<string, string>> Save(Stream fileStream, string fileName, string? directoryName = null)
        {
            Dictionary<string, string> result = new();

            // Директория хранения
            string fileDir = string.Empty; // Полный путь
            string relativePath = string.Empty; // Путь относительно медиа-директории
            if (string.IsNullOrEmpty(directoryName))
                fileDir = this._root;
            else
            {
                fileDir = Path.Combine(this._root, directoryName);
                relativePath = Path.Combine(directoryName);
            }

            //Новое уникальное имя
            string newName = await _getRandomUniqueFileName(fileName, directoryName);
            // Новый полный путь
            string newFullFilePAth = Path.GetFullPath(Path.Combine(fileDir, newName));

            long size;
            using (var stream = System.IO.File.OpenWrite(newFullFilePAth))
            {
                await fileStream.CopyToAsync(stream);
                size = stream.Length;
            }

            result.Add("originalFileName", fileName);
            result.Add("uniqueFileName", newName);
            result.Add("type", $"{Path.GetExtension(newName)}");
            result.Add("size", $"{size.ToString()}");
            result.Add("relativePath", Path.Combine(relativePath, newName));

            return result;
        }

        /// <summary>
        /// Генерирует уникальное имя.
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="directoryName">Директория</param>
        /// <returns>Уникальное имя</returns>
        private async Task<string> _getRandomUniqueFileName(string fileName, string? directoryName = null)
        {
            string newName = string.Empty;

            string randomName = Path.GetRandomFileName();
            var extension = Path.GetExtension(fileName);
            newName = Path.ChangeExtension(randomName, extension);

            if (await FileExist(newName, directoryName))
                newName = await _getRandomUniqueFileName(fileName, directoryName);

            return newName;
        }

        /// <summary>
        /// Удаляет файл.
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="directory">Директория</param>
        /// <returns>True- если успешно удалено, иначе false</returns>
        public async Task<bool> Delete(string fileName, string? directory = null)
        {
            await Task.CompletedTask;

            string fileDir = string.Empty;
            if (string.IsNullOrEmpty(directory))
                fileDir = this._root;
            else
                fileDir = Path.Combine(this._root, directory);

            string filePath = Path.Combine(fileDir, fileName);

            if (!File.Exists(filePath)) return false;

            File.Delete(filePath);
            return true;

        }

        /// <summary>
        /// Поверяет существует ли файл.
        /// </summary>
        /// <param name="fileName">Имя файла с расширением</param>
        /// <param name="directoryName">Директория</param>
        /// <returns>True- если существует, иначе false</returns>
        public async Task<bool> FileExist(string fileName, string? directoryName = null)
        {
            await Task.CompletedTask;

            string path = string.Empty;

            if (!string.IsNullOrEmpty(directoryName))
                path = Path.Combine(_root, directoryName, fileName);
            else
                path = Path.Combine(_root, fileName);

            return File.Exists(path);
        }


    }
}
