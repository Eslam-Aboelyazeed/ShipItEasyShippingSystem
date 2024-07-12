using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class Governorate
    {
        public int id { get; set; }
        public string name { get; set; }
        public Status status { get; set; }

        public List<City> cities { get; set; }
        public List<Merchant> governorateMerchants { get; set; }
        public List<GovernorateRepresentatives> representatives { get; set; }
        public List<Order> governorateOrders { get; set; }
        public List<SpecialPackages> specialPackages { get; set; }
    }
}
