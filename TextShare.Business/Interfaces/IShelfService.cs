using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;

namespace TextShare.Business.Interfaces
{
    public interface IShelfService
    {
        Task<List<Shelf>> GetAllShelvesAsync();
        Task<Shelf?> GetShelfByIdAsync(int id);
        Task<List<Shelf>> FindShelvesAsync(Expression<Func<Shelf, bool>> predicate);
        Task<Shelf> CreateShelfAsync(Shelf shelf);
        Task<bool> DeleteShelfAsync(int id);
        Task<Shelf> UpdateShelfAsync(Shelf shelf);
        Task<bool> ContainsShelfAsync(Shelf shelf);
    }
}
