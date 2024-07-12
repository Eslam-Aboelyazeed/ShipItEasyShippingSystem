using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork 
    {
        private readonly ShippingContext context;

        public UnitOfWork(ShippingContext context)
        {
            this.context = context;
        }

        public IGenericRepository<T> GetGenericRepository<T>() where T : class
        {
            return new GenericRepository<T>(context);
        }

        public IPaginationRepository<T> GetPaginationRepository<T>() where T : class
        {
            return new PaginationRepository<T>(context);
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception) { }

            return false;
        }
    }
}
