using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Models.EntityModels.UserModels;
using X.PagedList;

namespace TextShare.Domain.Models.EntityModels.GroupModels
{
    public class GroupMembersModel
    {
        public UserModel CurrentUser { get; set; }
        public GroupDetailModel Group{ get; set; }
        public IPagedList<UserModel> Members { get; set; }
    }
}
