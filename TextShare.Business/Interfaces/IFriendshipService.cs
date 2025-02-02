using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Interfaces
{
    public interface IFriendshipService
    {
        Task<List<Friendship>> GetAllFriendshipsAsync();
        Task<Friendship?> GetFriendshipByIdAsync(int id);
        Task<List<Friendship>> GetFriendshipsByUserIdAsync(int userId);
        Task<List<Friendship>> GetFriendshipsByFriendIdAsync(int friendId);
        Task<Friendship> CreateFriendshipAsync(Friendship friendship);
        Task<bool> DeleteFriendshipAsync(int id);
        Task<Friendship> UpdateFriendshipAsync(Friendship friendship);
        Task<bool> ContainsFriendshipAsync(Friendship friendship);
        Task<bool> ConfirmFriendshipAsync(int friendshipId);
        Task<bool> RejectFriendshipAsync(int friendshipId);
    }
}
