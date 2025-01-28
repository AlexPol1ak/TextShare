using Microsoft.AspNetCore.Identity;
using TextShare.Domain.Entities.AccessRules;
using TextShare.Domain.Entities.Complaints;
using TextShare.Domain.Entities.Groups;
using TextShare.Domain.Entities.TextFiles;


namespace TextShare.Domain.Entities.Users
{
    /// <summary>
    /// Класс пользователя
    /// </summary>
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Patronymic { get; set; }
        public DateOnly BirthDate { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public string? AvatarUri { get; set; }
        public string? SelfDescription { get; set; }

        // Коллекции для исходящих и входящих запросов дружбы
        public ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
        public ICollection<Friendship> FriendRequests { get; set; } = new List<Friendship>();

        //Коллекции для созданных групп и участия в группах
        public ICollection<Group> Groups { get; set; } = new List<Group>();
        public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();

        // Коллекция полок.
        public ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();

        // Коллекция файлов
        public ICollection<TextFile> TextFiles { get; set; } = new List<TextFile>();

        // Доступ к файлам
        public ICollection<AccessRule> AccessRules { get; set; } = new List<AccessRule>();

        // Жалобы
        public ICollection<Complaint> MyComplaints {  get; set; } = new  List<Complaint>(); 

        public override string ToString()
        {
            return $"Id: {Id}. First Name: {FirstName}. Last Name {LastName}.";
        }

        /// <summary>
        /// Полная информация о пользователе.
        /// </summary>
        /// <returns></returns>
        public string GetFullInfo()
        {
            string info = string.Empty;
            info += ToString() + " ";
            info += $"BirthDate: {BirthDate.ToShortDateString()}. " +
                $"RegisteredAt: {RegisteredAt.ToString()}\n";

            info += $"Number friends: {Friendships.Count.ToString()}. " +
                $"Number friend requests: {FriendRequests.Count.ToString()}\n";

            info += $"Number groups: {Groups.Count.ToString()}. " +
                $"Group member number: {GroupMemberships.Count.ToString()}\n";

            info += $"Number Shelves: {Shelves.Count.ToString()}. ";
            info += $"Number text files: {TextFiles.Count.ToString()}.\n";
            info += $"Number complaints: {MyComplaints.Count.ToString()}.\n";

            return info ;
        }
    }

}
