using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextShare.Domain.Settings
{
    /// <summary>
    /// Класс настроек для полок
    /// </summary>
    public class ShelvesSettings
    {
        public int MaxNumberShelvesInPage { get; set; }
        public int MaxNumberUserShelves { get; set; }
        public int MaxFilesInShelf { get; set; }
    }
}
