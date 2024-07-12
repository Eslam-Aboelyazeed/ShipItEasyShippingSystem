using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.DisplayDTOs
{
    public class SettingsDTO
    {
        public int Id { get; set; }
        public decimal BaseWeight { get; set; }
        public decimal AdditionalFeePerKg { get; set; }
        public decimal VillageDeliveryFee { get; set; }
        public decimal OrdinaryShippingCost { get; set; }
        public decimal TwentyFourHoursShippingCost { get; set; }
        public decimal FifteenDayShippingCost { get; set; }
    }
}
