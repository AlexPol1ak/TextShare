using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextShare.Domain.Settings
{
    /// <summary>
    /// Класс настроек группы.
    /// </summary>
    public class GroupsSettings
    {
        public int MaxGroupsCreate { get; set; }
        public int MaxUserGroups { get; set; }
        public int MaxGroupInPage { get; set; }
    }
}
