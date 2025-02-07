using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Services
{
    public class ShelfService : IShelfService
    {
        private readonly IRepository<Shelf> _repositoryShelves;

        public ShelfService(IUnitOfWork unitOfWork)
        {
            _repositoryShelves = unitOfWork.ShelfRepository;
        }

        public async Task<Shelf> AddCreatorShelfAsync(Shelf shelf, User user)
        {
            bool shelfExist = await ContainsShelfAsync(shelf);
            Shelf responseShelf;

            shelf.Creator = user;
            if(shelfExist) responseShelf =  await UpdateShelfAsync(shelf);
            else responseShelf = await CreateShelfAsync(shelf);

            return responseShelf;
        }

        public async Task<bool> ContainsShelfAsync(Shelf shelf)
        {
            return await _repositoryShelves.ContainsAsync(shelf);
        }

        public async Task<Shelf> CreateShelfAsync(Shelf shelf)
        {
            return await _repositoryShelves.CreateAsync(shelf);
        }

        public async Task<bool> DeleteShelfAsync(int id)
        {
            return await _repositoryShelves.DeleteAsync(id);
        }

        public async Task<List<Shelf>> FindShelvesAsync(Expression<Func<Shelf, bool>> predicate)
        {
            return await _repositoryShelves.FindAsync(predicate);
        }

        public async Task<List<Shelf>> GetAllShelvesAsync()
        {
            return await _repositoryShelves.GetAllAsync();
        }

        public async Task<Shelf?> GetShelfByIdAsync(int id)
        {
            return await _repositoryShelves.GetAsync(id);
        }

        public async Task<Shelf> UpdateShelfAsync(Shelf shelf)
        {
            return await _repositoryShelves.UpdateAsync(shelf);
        }
    }
}
