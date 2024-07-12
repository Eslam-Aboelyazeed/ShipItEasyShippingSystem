using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public string? StatusNote { get; set; }

        [ForeignKey("order")]
        public int OrderId { get; set; }

        public OrderStatus ProductStatus { get; set; }
        public Order order { get; set; }    
    }
}
