using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Entities.Groups
{
    public class GroupMember
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime JoinedAt { get; set; }
        public bool IsConfirmed { get; set; } = false;
    }
}
