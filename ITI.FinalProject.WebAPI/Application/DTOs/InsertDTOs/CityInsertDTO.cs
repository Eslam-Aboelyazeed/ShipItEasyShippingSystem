using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class CityInsertDTO
    {
        public string name { get; set; }
        public Status status { get; set; }
        public decimal normalShippingCost { get; set; }
        public decimal pickupShippingCost { get; set;}
        public int governorateId { get; set; }

    }
}
