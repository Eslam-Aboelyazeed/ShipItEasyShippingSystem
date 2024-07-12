using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class PaginationDTO<T> where T : class
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<T> List { get; set; }
    }
}
