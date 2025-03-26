using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextShare.Domain.Entities.TextFiles;
using TextShare.Domain.Entities.Users;

namespace TextShare.Domain.Models.EntityModels.ShelfModels
{
    /// <summary>
    /// Модель отображения краткой информации о полке.
    /// </summary>
    public class ShelfDetailShort
    {
        public int ShelfId { get; set; }
        public string Name { get; set; }
        public string? AvatarUri { get; set; }

        public User Creator { get; set; }

        public async static Task<ShelfDetailShort> FromShelf( Shelf shelf)
        {
            await Task.CompletedTask;
            ShelfDetailShort sh = new();
            sh.ShelfId = shelf.ShelfId;
            sh.Name = shelf.Name;
            sh.AvatarUri = shelf.ImageUri;
            sh.Creator = shelf.Creator;
            return sh;
        }
        
        public async static Task<List<ShelfDetailShort>> FromShelves(IEnumerable<Shelf> shelves)
        {
            var tasks = shelves.Select(s=> FromShelf(s)).ToList();
            var shelvesModels =  await Task.WhenAll(tasks);
            return shelvesModels.ToList();
        }
    }
}
