using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.InsertDTOs
{
    public class SpecialPackageInsertDTO
    {
        public decimal ShippingPrice { get; set; }
        public int cityId { get; set; }
        public int governorateId { get; set; }
    }
}
