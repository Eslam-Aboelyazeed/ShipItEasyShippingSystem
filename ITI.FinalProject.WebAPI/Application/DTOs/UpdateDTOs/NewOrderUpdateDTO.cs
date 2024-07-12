using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UpdateDTOs
{
    public class NewOrderUpdateDTO
    {
        public int Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string? RepresentativeId { get; set; }
    }
}
