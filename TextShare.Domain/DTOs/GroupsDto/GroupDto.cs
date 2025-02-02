﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TextShare.Domain.Entities.Groups;

namespace TextShare.Domain.DTOs.GroupsDto
{
    /// <summary>
    /// DTO-класс для группы пользователей.
    /// </summary>
    public class GroupDto
    {
        public int GroupId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ImageUri { get; set; }

        [Required]
        public int CreatorId { get; set; }

        public static GroupDto FromGroup(Group group)
        {
            return new GroupDto
            {
                GroupId = group.GroupId,
                Name = group.Name,
                Description = group.Description,
                CreatedAt = group.CreatedAt,
                ImageUri = group.ImageUri,
                CreatorId = group.CreatorId
            };
        }

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
