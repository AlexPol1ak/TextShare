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

        /// <summary>
        /// Создает экземпляр <see cref="GroupDeleteModel"/> на основе сущности <see cref="Group"/>.
        /// </summary>
        /// <param name="group">Сущность группы.</param>
        /// <returns>Объект <see cref="GroupDeleteModel"/>.</returns>
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
