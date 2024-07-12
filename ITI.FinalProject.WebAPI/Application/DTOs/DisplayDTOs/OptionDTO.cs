using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class OptionDTO<T>
    {
        public T Id { get; set; }
        public string Name { get; set; }
        public int? DependentId { get; set; }
        public List<int>? DependentIds { get; set; }
    }
}
