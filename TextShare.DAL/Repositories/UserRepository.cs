using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TextShare.DAL.Data;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.Users;

namespace TextShare.DAL.Repositories
{
    public class UserRepository : IRepository<User>
    {

        private readonly DbSet<User> _users;

        public UserRepository(TextShareContext context)
        {
            _users = context.Users;
        }

        public Task<bool> Contains(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> Count()
        {
            throw new NotImplementedException();
        }

        public Task<User> Create(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<User>> Find(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetAsync(int id, params string[] includes)
        {
            throw new NotImplementedException();
        }

        public Task<User> Update(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
