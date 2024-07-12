using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UpdateDTOs
{
    public class SpecialPackageUpdateDTO
    {
        public decimal ShippingPrice { get; set; }
        public int cityId { get; set; }
        public int governorateId { get; set; }
        public string MerchantId { get; set; }
    }
}
