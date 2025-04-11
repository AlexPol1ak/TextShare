using Microsoft.EntityFrameworkCore;
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
            var query = await _repositoryFriendships.GetAllAsync(includes);
            return await query.ToListAsync();
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

        public async Task<List<Friendship>> GetAllUserAcceptedFriendshipAsync(int userId,
            params Expression<Func<Friendship, object>>[] includes)
        {
            List<Friendship> friendships = (await FindFriendshipsAsync(
                f => (f.UserId == userId || f.FriendId == userId) && f.IsConfirmed == true, includes
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
            var query = await _repositoryFriendships.FindAsync(predicate, includes);
            return await query.ToListAsync();
        }

        public async Task<List<User>> GetOutFriendRequestsUsers(int userId,
            params Expression<Func<User, object>>[] includes)
        {
            IQueryable<Friendship> outRequestsQuery = await _repositoryFriendships.FindAsync(
                f => f.UserId == userId && f.IsConfirmed == false, f => f.Friend
                );
            IQueryable<User> usersQuery = outRequestsQuery.Select(f => f.Friend);

            foreach (var include in includes)
            {
                usersQuery = usersQuery.Include(include);
            }

            return await usersQuery.ToListAsync();
        }

        public async Task<List<User>> GetInFriendRequestsUsers(int userId,
            params Expression<Func<User, object>>[] includes)
        {
            IQueryable<Friendship> outRequestsQuery = await _repositoryFriendships.FindAsync(
                f => f.FriendId == userId && f.IsConfirmed == false, f => f.User
                );
            IQueryable<User> usersQuery = outRequestsQuery.Select(f => f.User);

            foreach (var include in includes)
            {
                usersQuery = usersQuery.Include(include);
            }

            return await usersQuery.ToListAsync();
        }

        public async Task<List<User>> GetFriendsUser(int userId, params Expression<Func<User, object>>[] includes)
        {
            IQueryable<Friendship> friendshipsQuery = await _repositoryFriendships.FindAsync(
                f => (f.UserId == userId || f.FriendId == userId) && f.IsConfirmed == true,
                f => f.Friend, f => f.User
                );

            IQueryable<User> usersQuery = friendshipsQuery
                    .Select(f => f.User) // Выбираем одного пользователя
                    .Union(friendshipsQuery.Select(f => f.Friend)) // Объединяем с другим
                    .Distinct()
                    .Where(user => user.Id != userId);

            foreach (var include in includes)
            {
                usersQuery = usersQuery.Include(include);
            }

            return await usersQuery.ToListAsync();
        }
    }
}
