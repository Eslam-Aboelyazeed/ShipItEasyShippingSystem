using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SpecialPackages
    {
        //public int Id { get; set; }
        [Column(TypeName = "money")]
        public decimal ShippingPrice { get; set; }
        [ForeignKey("cityPackages")]
        public int cityId { get; set; }
        [ForeignKey("governoratePackages")]
        public int governorateId { get; set; }
        [ForeignKey("merchantSpecialPackage")]
        public string MerchantId { get; set; }
        public Governorate governoratePackages { get; set; }
        public City cityPackages { get; set; }
        public Merchant merchantSpecialPackage { get; set; }

    }
}
