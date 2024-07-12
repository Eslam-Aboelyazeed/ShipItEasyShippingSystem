using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;


namespace Domain.Entities
{
    public class Branch
    {
        public int id { get; set; }
        public string name { get; set; }
        public Status status { get; set; }
        public DateTime addingDate { get; set; }
        [ForeignKey("city")]
        public int cityId { get; set; }
        
        public City city { get; set; }
        public List<ApplicationUser> users { get; set; }
        public List<Order> branchOrders { get; set; }
    }
}
