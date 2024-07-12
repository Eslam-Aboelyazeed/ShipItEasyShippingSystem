using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<List<T>> GetAllElements();

        public Task<List<T>> GetAllElements(Expression<Func<T, bool>> filter);

        public Task<List<T>> GetAllElements(params Expression<Func<T, object>>[] includes);

        public Task<List<T>> GetAllElements(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

        public Task<List<T>> GetElementsWithoutTracking();
        
        public Task<List<T>> GetElementsWithoutTracking(Expression<Func<T, bool>> filter);
        
        public Task<List<T>> GetElementsWithoutTracking(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

        public Task<T?> GetElement(Expression<Func<T, bool>> filter);

        public Task<T?> GetElement(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

        public Task<T?> GetElementWithoutTracking(Expression<Func<T, bool>> filter);

        public Task<T?> GetElementWithoutTracking(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);

        public bool Add(T element);

        public bool Edit(T element);

        public bool Delete(T element);  
    }
}
