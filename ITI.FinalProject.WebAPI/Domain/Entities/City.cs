using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public Status status { get; set; }
        [Column(TypeName = "money")]
        public decimal normalShippingCost { get; set; }
        [Column(TypeName = "money")]
        public decimal pickupShippingCost { get; set;}
        [ForeignKey("governorate")]
        public int governorateId { get; set; }

        public Branch branch { get; set; }
        public Governorate governorate { get; set; }
        public List<SpecialPackages> citySpecialPackages { get; set; }
        public List<Merchant> cityMerchants { get; set; }
        public List<Order> cityOrders { get; set; }

    }
}
