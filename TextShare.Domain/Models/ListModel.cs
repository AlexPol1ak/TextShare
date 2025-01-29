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

        /// <summary>
        /// Выполняет пагинацию последовательности.
        /// </summary>
        /// <typeparam name="I">Тип элементов</typeparam>
        /// <param name="fullItemsList">Последовательность элементов</param>
        /// <param name="currentPage">Страница элементов</param>
        /// <param name="pageSize">Количество элементов на странице</param>
        /// <returns></returns>
        public async static Task<ListModel<I>> GetItemsPart<I>(List<I> fullItemsList, int currentPage, int pageSize)
        {
            await Task.CompletedTask;

            ListModel<I> listModel = new();
            int totalItems = fullItemsList.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (currentPage > totalPages && totalPages > 0)
                currentPage = totalPages;

            var items = fullItemsList
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            listModel.Items = items;
            listModel.CurrentPage = currentPage;
            listModel.TotalPages = totalPages;

            return listModel;
        }
    }
}
