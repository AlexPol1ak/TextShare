using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Services
{
    /// <summary> 
    /// Сервис для управления пользователями.
    /// </summary>
    public class UserService : BaseService, IUserService
    {
        private readonly IRepository<User> _repositoryUsers;

        public UserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repositoryUsers = unitOfWork.UserRepository;
        }

        public async Task<bool> ContainsUserAsync(User user)
        {
            return await _repositoryUsers.ContainsAsync(user);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await _repositoryUsers.CreateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _repositoryUsers.DeleteAsync(id);
        }

        public async Task<List<User>> FindUsersAsync(Expression<Func<User, bool>> predicate,
            params Expression<Func<User, object>>[] includes)
        {
            var query = await _repositoryUsers.FindAsync(predicate, includes);
            return await query.ToListAsync() ;
        }

        public async Task<List<User>> GetAllUsersAsync(params Expression<Func<User, object>>[] includes)
        {
            var query = await _repositoryUsers.GetAllAsync(includes);
            return await query.ToListAsync() ;
        }

        public async Task<User?> GetUserByIdAsync(int id, params Expression<Func<User, object>>[] includes)
        {
            return await _repositoryUsers.GetAsync(id, includes);
        }

        public async Task<User?> GetUserByUsernameAsync(string username, params Expression<Func<User, object>>[] includes)
        {
            List<User> users = await  (await _repositoryUsers.FindAsync(u => u.UserName == username, includes)).ToListAsync();       
            return users.FirstOrDefault();
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            return await _repositoryUsers.UpdateAsync(user);
        }
    }
}
