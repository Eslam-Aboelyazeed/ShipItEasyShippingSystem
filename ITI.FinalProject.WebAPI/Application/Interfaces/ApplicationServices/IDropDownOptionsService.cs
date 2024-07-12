using Application.DTOs.DisplayDTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ApplicationServices
{
    public interface IDropDownOptionsService<T1,T2> where T1 : class
    {
        public Task<List<OptionDTO<T2>>> GetOptions(params Expression<Func<T1, object>>[] includes);
        public Task<List<OptionDTO<T2>>> GetOptions(Expression<Func<T1, bool>> filter, params Expression<Func<T1, object>>[] includes);

    }
}
