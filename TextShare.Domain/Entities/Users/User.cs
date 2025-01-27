using Microsoft.AspNetCore.Identity;


namespace TextShare.Domain.Entities.Users
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Patronymic { get; set; }
        public DateOnly BirthDate { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public string? AvatarUri { get; set; }
        public string? SelfDescription { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}. First Name: {FirstName}. Last Name {LastName}";
        }
    }

}
