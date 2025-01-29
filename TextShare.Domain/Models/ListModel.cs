using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextShare.Domain.Models
{
    /// <summary>
    /// Класс для коллекции ответов.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListModel<T>
    {
        public List<T> Items { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;

        public ListModel () { }
        public ListModel(List<T> items, int currentPage, int totalPages)
        {
            Items = items;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }

        public override string ToString()
        {
            string str = $"ListModel: Count items - {Items.Count}. " +
                $"Current Page - {CurrentPage}. Total Pages - {TotalPages}.";
            return str;
        }
    }
}
