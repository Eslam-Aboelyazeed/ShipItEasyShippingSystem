using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class CityDisplayDTO
    {
        public int id {  get; set; }
        public string name { get; set; }
        public Status status { get; set; }
        public decimal normalShippingCost { get; set; }
        public decimal pickupShippingCost { get; set; }
        public int governorateId { get; set; }
        
    }
}
