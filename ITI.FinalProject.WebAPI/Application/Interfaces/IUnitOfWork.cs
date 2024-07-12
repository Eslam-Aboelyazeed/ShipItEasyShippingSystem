using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUnitOfWork
    {
        public IGenericRepository<T> GetGenericRepository<T>() where T : class;
        public Task<bool> SaveChanges();
        public IPaginationRepository<T> GetPaginationRepository<T>() where T : class;
    }
}
