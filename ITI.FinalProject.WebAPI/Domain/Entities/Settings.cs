using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Settings
    {
        public int Id { get; set; }
        public decimal BaseWeight { get; set; }
        [Column(TypeName = "money")]
        public decimal AdditionalFeePerKg { get; set; }
        [Column(TypeName = "money")]
        public decimal VillageDeliveryFee { get; set; }
        [Column(TypeName = "money")]
        public decimal OrdinaryShippingCost { get; set; }
        [Column(TypeName = "money")]
        public decimal TwentyFourHoursShippingCost { get; set; }
        [Column(TypeName = "money")]
        public decimal FifteenDayShippingCost { get; set; }

    }
}
