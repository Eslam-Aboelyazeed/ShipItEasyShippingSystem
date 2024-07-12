using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ShippingContext db;

        public GenericRepository(ShippingContext db)
        {
            this.db = db;
        }

        public async Task<List<T>> GetAllElements()
        {
            return await db.Set<T>().ToListAsync();
        }

        public async Task<List<T>> GetAllElements(Expression<Func<T, bool>> filter)
        {
            return await db.Set<T>().Where(filter).ToListAsync();
        }

        public async Task<List<T>> GetAllElements(params Expression<Func<T, object>>[] includes)
        {
            var query = db.Set<T>().AsQueryable();

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.ToListAsync();
        }

        public async Task<List<T>> GetAllElements(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var query = db.Set<T>().Where(filter);

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.ToListAsync();
        }

        public async Task<List<T>> GetElementsWithoutTracking()
        {
            return await db.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<List<T>> GetElementsWithoutTracking(Expression<Func<T, bool>> filter)
        {
            return await db.Set<T>().AsNoTracking().Where(filter).ToListAsync();
        }

        public async Task<List<T>> GetElementsWithoutTracking(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var element = db.Set<T>().Where(filter);

            var query = element.AsQueryable();

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetElement(Expression<Func<T, bool>> filter)
        {
            return await db.Set<T>().FirstOrDefaultAsync(filter);
        }

        public async Task<T?> GetElement(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var element = db.Set<T>().Where(filter);

            var query = element.AsQueryable();

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T?> GetElementWithoutTracking(Expression<Func<T, bool>> filter)
        {
            return await db.Set<T>().AsNoTracking().FirstOrDefaultAsync(filter);
        }

        public async Task<T?> GetElementWithoutTracking(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var element = db.Set<T>().Where(filter);

            var query = element.AsQueryable();

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public bool Add(T element)
        {
            try
            {
                db.Entry(element).State = EntityState.Added;

                return true;
            }
            catch (Exception) { }

            return false;
        }

        public bool Edit(T element)
        {
            try
            {
                db.Entry(element).State = EntityState.Modified;

                return true;
            }
            catch (Exception) { }

            return false;
        }

        public bool Delete(T element)
        {
            try
            {
                db.Entry(element).State = EntityState.Deleted;

                return true;
            }
            catch (Exception) { }

            return false;
        }
    }
}
