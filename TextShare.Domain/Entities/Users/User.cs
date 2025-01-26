using Microsoft.AspNetCore.Identity;


namespace TextShare.Domain.Entities.Users
{
    public class User : IdentityUser
    {
        public string? AvatarUri { get; set; }
    }
}
