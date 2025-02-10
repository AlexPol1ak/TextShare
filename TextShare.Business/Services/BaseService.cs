using System.Linq.Expressions;
using TextShare.Business.Interfaces;
using TextShare.DAL.Interfaces;

namespace TextShare.Business.Services
{
    /// <summary>
    /// Базовый сервис
    /// </summary>
    public abstract class BaseService : IBaseService
    {
        private readonly IUnitOfWork _unitOfWork;

        protected BaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task LoadRelatedEntitiesAsync<T, TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> navigationProperty)
            where T : class
            where TProperty : class
        {
            await _unitOfWork.LoadRelatedEntitiesAsync<T, TProperty>(entity, navigationProperty);
        }

        public async Task<int> SaveAsync()
        {
            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
