using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Business.Services
{

    /// <summary> 
    ///  Сервис для управления полками
    /// </summary>
    public class ShelfService : BaseService,  IShelfService
    {
        private readonly IRepository<Shelf> _repositoryShelves;

        public ShelfService(IUnitOfWork unitOfWork) :base(unitOfWork)
        {
            _repositoryShelves = unitOfWork.ShelfRepository;          
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

        public async Task<List<Shelf>> GetAllShelvesAsync(params string[] includes)
        {
            return await _repositoryShelves.GetAllAsync(includes);
        }

        public async Task<Shelf?> GetShelfByIdAsync(int id, params string[] includes)
        {
            return await _repositoryShelves.GetAsync(id, includes);
        }

        public async Task<Shelf> UpdateShelfAsync(Shelf shelf)
        {
            return await _repositoryShelves.UpdateAsync(shelf);
        }
    }
}
