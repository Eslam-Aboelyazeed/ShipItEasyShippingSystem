using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.InsertDTOs
{
    public class InsertProductDTO
    {
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? StatusNote { get; set; }
        public int OrderId { get; set; }
        public OrderStatus ProductStatus { get; set; }
    }
}
