using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class SpecialPackageDTO
    {
        public decimal ShippingPrice { get; set; }
        public string cityName { get; set; }
        public string governorateName { get; set; }
        public string MerchantName { get; set; }
    }
}
