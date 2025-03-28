using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Groups;

namespace TextShare.Domain.Models.EntityModels.GroupModels
{
    /// <summary>
    /// Модель для группы пользователей.
    /// </summary>
    public class GroupModel
    {
        public int GroupId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ImageUri { get; set; }

        [Required]
        public int CreatorId { get; set; }

        /// <summary>
        /// Создает объект <see cref="GroupModel"/> из сущности <see cref="Group"/>.
        /// </summary>
        /// <param name="group">Сущность группы.</param>
        /// <returns>Модель группы.</returns>
        public static GroupModel FromGroup(Group group)
        {
            return new GroupModel
            {
                GroupId = group.GroupId,
                Name = group.Name,
                Description = group.Description,
                CreatedAt = group.CreatedAt,
                ImageUri = group.ImageUri,
                CreatorId = group.CreatorId
            };
        }

        /// <summary>
        /// Преобразует модель группы в сущность <see cref="Group"/>.
        /// </summary>
        /// <returns>Сущность группы.</returns>
        public Group ToGroup()
        {
            return new Group
            {
                GroupId = GroupId,
                Name = Name,
                Description = Description,
                CreatedAt = CreatedAt,
                ImageUri = ImageUri,
                CreatorId = CreatorId
            };
        }
    }
}
