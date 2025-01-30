using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TextShare.Business.Interfaces;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Utils;
using static System.Net.Mime.MediaTypeNames;

namespace TextShare.Business.Services
{
    /// <summary>
    /// Класс физического управления файлом.
    /// </summary>
    public class PhysicalFileService : IPhysicalFile
    {
        private readonly string _root;

        public PhysicalFileService(string root)
        {
            this._root = root;
        }

        public Task<Uri?> Get(string fileName, string? directoryName = null)
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<string, string>> Save(Stream fileStream, string fileName, string? directoryName = null)
        {
            Dictionary<string, string> result = new ();

            // Директория хранения
            string fileDir = string.Empty;
            if (string.IsNullOrEmpty(directoryName))
                fileDir = this._root;
            else
                fileDir = Path.Combine(this._root, directoryName);
                

            string newName = await _getRandomUniqueFileName(fileName, directoryName);
            string newFullFilePAth = Path.GetFullPath(Path.Combine(fileDir, newName));
           
            using var stream = System.IO.File.OpenWrite(newFullFilePAth);
            await fileStream.CopyToAsync(stream);

            long size = stream.Length;

            result.Add("originalName", fileName);
            result.Add("uniqueName", newName);
            result.Add("type", $"{Path.GetExtension(newName)}");
            result.Add("size", $"{size.ToString()}");

            return result;
        }

        private async Task<string> _getRandomUniqueFileName(string fileName, string? directoryName = null)
        {
            string newName = string.Empty;

            string randomName = Path.GetRandomFileName();
            var extension = Path.GetExtension(fileName);
            newName = Path.ChangeExtension(randomName, extension);

            if(await FileExist(newName, directoryName))
               newName = await  _getRandomUniqueFileName(fileName, directoryName);

            return newName;
        }

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
