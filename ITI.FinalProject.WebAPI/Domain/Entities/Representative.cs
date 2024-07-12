using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Representative
    {
        public DeductionType DiscountType { get; set; }
        public double CompanyPercentage{ get; set; }
        [ForeignKey("user")]
        [Key]
        public string userId { get; set; }

        public ApplicationUser user { get; set; }
        public List<GovernorateRepresentatives> governorates { get; set; }
        public List<Order> representativeOrders { get; set; }
    }
}
