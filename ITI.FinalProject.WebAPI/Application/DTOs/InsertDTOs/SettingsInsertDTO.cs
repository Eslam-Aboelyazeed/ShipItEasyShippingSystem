using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.InsertDTOs
{
    public class SettingsInsertDTO
    {
        public decimal BaseWeight { get; set; }
        public decimal AdditionalFeePerKg { get; set; }
        public decimal VillageDeliveryFee { get; set; }
        public decimal OrdinaryShippingCost { get; set; }
        public decimal TwentyFourHoursShippingCost { get; set; }
        public decimal FifteenDayShippingCost { get; set; }
    }
}
