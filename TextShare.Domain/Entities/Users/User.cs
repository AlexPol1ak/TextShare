using Microsoft.AspNetCore.Identity;


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

        public override string ToString()
        {
            return $"Id: {Id}. First Name: {FirstName}. Last Name {LastName}";
        }
    }

}
