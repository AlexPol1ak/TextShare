using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Business.Services
{
    /// <summary>
    /// Класс физического управления файлом.
    /// </summary>
    public class PhysicalFileService : IPhysicalFile
    {
        private readonly string _root;
        private readonly string _host;
        private readonly string _schema;

        public PhysicalFileService(string root, string host, string schema)
        {
            this._root = root;
            this._host = host;
            this._schema = schema;
        }

        public Task<Uri?> Get(string fileName, string? directory = null)
        {
            throw new NotImplementedException();
        }

        public Task<Dictionary<string, string>> Save(Stream fileStream, string fileName, string? directory = null)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(string fileName, string? directory = null)
        {
            await Task.CompletedTask;

            string path = string.Empty;

            if (!string.IsNullOrEmpty(directory))
                path = Path.Combine(_root, directory, fileName);
            else
                path = Path.Combine(_root, fileName);

            if (!File.Exists(path)) return false;

            File.Delete(path);
            return true;
   
        }

        public async Task<bool> FileExist(string fileName, string? directory = null)
        {
            await Task.CompletedTask;

            string path = string.Empty;

            if (!string.IsNullOrEmpty(directory))
                path = Path.Combine(_root, directory, fileName);
            else
                path = Path.Combine(_root, fileName);

            return File.Exists(path);
        }

        
    }
}
