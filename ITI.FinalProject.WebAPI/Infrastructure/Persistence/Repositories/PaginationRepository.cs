using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class PaginationRepository<T> : GenericRepository<T>, IPaginationRepository<T> where T : class
    {
        private readonly ShippingContext _context;

        public PaginationRepository(ShippingContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetPaginatedElements(int pageNumber, int pageSize, Expression<Func<T, bool>> filter)
        {
            return await _context.Set<T>()
                                 .Where(filter)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetPaginatedElements(int pageNumber, int pageSize, Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().Where(filter);

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();
        }


        public async Task<int> Count()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<int> Count(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().Where(filter);

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.CountAsync();
        }

        public async Task<int> Pages(int pageSize)
        {
            var list = await GetAllElements();

            return (int)Math.Ceiling((double)(list.Count) / pageSize);
        }
    }
}
