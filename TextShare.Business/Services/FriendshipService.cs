using System.Linq.Expressions;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Services
{
    /// <summary> 
    ///  Сервис для управления дружбой между пользователями
    /// </summary>
    public class FriendshipService : BaseService, IFriendshipService
    {
        private readonly IRepository<Friendship> _repositoryFriendships;

        public FriendshipService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repositoryFriendships = unitOfWork.FriendshipRepository;
        }

        public async Task<List<Friendship>> GetAllFriendshipsAsync(params Expression<Func<Friendship, object>>[] includes)
        {
            return await _repositoryFriendships.GetAllAsync(includes);
        }
        public async Task<bool> ContainsFriendshipAsync(Friendship friendship)
        {
            return await _repositoryFriendships.ContainsAsync(friendship);
        }

        public async Task<Friendship> CreateFriendshipAsync(Friendship friendship)
        {
            return await _repositoryFriendships.CreateAsync(friendship);
        }

        public async Task<bool> DeleteFriendshipAsync(int id)
        {
            return await _repositoryFriendships.DeleteAsync(id);
        }
      
        public async Task<Friendship?> GetFriendshipByIdAsync(int id,
            params Expression<Func<Friendship, object>>[] includes)
        {
            return await _repositoryFriendships.GetAsync(id, includes);
        }

        public async Task<List<Friendship>> GetAllUserAcceptedFriendshipAsync(int userId, params Expression<Func<Friendship, object>>[] includes)
        {
            List<Friendship> friendships = (await FindFriendshipsAsync(
                f =>(f.UserId == userId || f.FriendId == userId) && f.IsConfirmed == true, includes
                ));

            return friendships;
        }
             
        public async Task<Friendship> UpdateFriendshipAsync(Friendship friendship)
        {
            return await _repositoryFriendships.UpdateAsync(friendship);
        }

        public async Task<bool> ConfirmFriendshipAsync(int friendshipId)
        {
            var friendship = await _repositoryFriendships.GetAsync(friendshipId);
            if (friendship != null && !friendship.IsConfirmed)
            {
                friendship.IsConfirmed = true;
                await _repositoryFriendships.UpdateAsync(friendship);
                return true;
            }
            return false;
        }

        public async Task<bool> RejectFriendshipAsync(int friendshipId)
        {
            return await _repositoryFriendships.DeleteAsync(friendshipId);
        }

        public async Task<List<Friendship>> FindFriendshipsAsync(Expression<Func<
            Friendship, bool>> predicate,
            params Expression<Func<Friendship, object>>[] includes)
        {
            return await _repositoryFriendships.FindAsync(predicate, includes);
        }

        public async Task<List<User>> GetOutFriendRequestsUsers(int userId)
        {
            IEnumerable<User> outRequests = (await FindFriendshipsAsync(
               f => f.UserId == userId && f.IsConfirmed == false, f => f.Friend
               )).Select(u => u.Friend);

            return outRequests.ToList();
        }
    }
}
