using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UpdateDTOs
{
    public class DeliveredOrderUpdateDTO
    {
        public int Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal? OrderMoneyReceived { get; set; }
        public decimal? ShippingMoneyReceived { get; set; }
        public string? Notes { get; set; }
    }
}
