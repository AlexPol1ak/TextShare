using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;
using X.PagedList;

namespace TextShare.Domain.Models.EntityModels.FriendsModels
{
    public class FriendSearchResultModel
    {
        public User User { get; set; }
        public List<User> Friends { get; set; } = new();
        public List<User> FriendRequests { get; set; } = new();
        public IPagedList<User> ResultSearch { get; set; } 
    }
}
