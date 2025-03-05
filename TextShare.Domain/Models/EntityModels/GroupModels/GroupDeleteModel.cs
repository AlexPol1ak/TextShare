using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Groups;

namespace TextShare.Domain.Models.EntityModels.GroupModels
{
    /// <summary>
    /// Модель для запроса удаления группы.
    /// </summary>
    public class GroupDeleteModel
    {
        public int GroupId { get; set; }
        public string Name { get; set; }

        public int CountMembers { get; set; }

        public static GroupDeleteModel FromGroup(Group group)
        {
            GroupDeleteModel groupDeleteModel = new();
            groupDeleteModel.GroupId = group.GroupId;
            groupDeleteModel.Name = group.Name;

            if (group.Members != null)
                groupDeleteModel.CountMembers = group.Members.Count;

                return groupDeleteModel;
        }
    }
}
